using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class BasicEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        KnockBack,
        Dead
    }

    private State currentState;

    [SerializeField] private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        knockBackDuration,
        lastTouchDamageTime,
        touchDamageCooldown,
        touchDamage,
        touchDamageHeight,
        touchDamageWidth;
    [SerializeField] private Transform
        groundCheck,
        wallCheck,
        touchDamageCheck;
    [SerializeField] private LayerMask
        whatIsGround,
        whatIsPlayer;
    [SerializeField] private Vector2 knockBackSpeed;

    [SerializeField] private GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;

    private int
        facingDirection,
        damageDirection;

    private float
        currentHealth,
        knockBackStartTime;

    private float[] atttackDetails = new float[2];

    private Vector2
        movement,
        touchDamageBottomLeft,
        touchDamageTopRight;
    
    private bool
        groundDetected,
        wallDetected;

    private GameObject _aliveGameObject;

    private Rigidbody2D _aliveRigidbody2D;

    private Animator _aliveAnimator;

    private void Start()
    {
        _aliveGameObject = transform.Find("Alive").gameObject;
        _aliveRigidbody2D = _aliveGameObject.GetComponent<Rigidbody2D>();
        _aliveAnimator = _aliveGameObject.GetComponent<Animator>();

        currentHealth = maxHealth;
        facingDirection = 1;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.KnockBack:
                UpdateKnockBackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    //--Moving State-----------------------------------------------

    private void EnterMovingState()
    {
        
    }

    private void UpdateMovingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);

        CheckTouchDamage();

        if (!groundDetected || wallDetected)
        {
            //Flip
            Flip();
        }
        else
        {
            //Move
            movement.Set(movementSpeed * facingDirection, _aliveRigidbody2D.velocity.y);
            _aliveRigidbody2D.velocity = movement;
        }
    }

    private void ExitMovingState()
    {
        
    }
    
    //--KnockBack State---------------------------------------------
    
    private void EnterKnockBackState()
    {
        knockBackStartTime = Time.time;
        movement.Set(knockBackSpeed.x * damageDirection, knockBackSpeed.y);
        _aliveRigidbody2D.velocity = movement;
        _aliveAnimator.SetBool("knockBack",true);
    }

    private void UpdateKnockBackState()
    {
        if (Time.time >= knockBackStartTime + knockBackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnockBackState()
    {
        _aliveAnimator.SetBool("knockBack", false);
    }
    
    //--Dead State--------------------------------------------------
    
    private void EnterDeadState()
    {
        //Spawn Chunks and Blood
        Instantiate(deathChunkParticle, _aliveGameObject.transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, _aliveGameObject.transform.position, deathBloodParticle.transform.rotation);
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {
        
    }

    private void ExitDeadState()
    {
        
    }
    
    //--Other Functions---------------------------------------------

    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        Instantiate(hitParticle, _aliveGameObject.transform.position,
            quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        
        if (attackDetails[1] > _aliveGameObject.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }
        
        //Hit Particle

        if (currentHealth > 0.0f)
        {
            SwitchState(State.KnockBack);
        }
        else if(currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    private void CheckTouchDamage()
    {
        if (Time.time >= lastTouchDamageTime + touchDamageCooldown)
        {
            touchDamageBottomLeft.Set(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
            touchDamageTopRight.Set(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));

            Collider2D hit = Physics2D.OverlapArea(touchDamageBottomLeft, touchDamageTopRight, whatIsPlayer);

            if (hit != null)
            {
                lastTouchDamageTime = Time.time;
                atttackDetails[0] = touchDamage;
                atttackDetails[1] = _aliveGameObject.transform.position.x;
                hit.SendMessage("Damage", atttackDetails);
            }
        }
    }
    
    private void Flip()
    {
        facingDirection *= -1;
        _aliveGameObject.transform.Rotate(0.0f,180.0f,0.0f);
    }
    
    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.KnockBack:
                ExitKnockBackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }
        
        switch (state)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.KnockBack:
                EnterKnockBackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

        Vector2 bottomLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
        Vector2 bottomRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));;
        Vector2 topLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));;
        Vector2 topRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));;

        Gizmos.DrawLine(bottomLeft,bottomRight);
        Gizmos.DrawLine(bottomRight,topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft,bottomLeft);


    }
}
