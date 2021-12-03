using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class Entity : MonoBehaviour
{
    public FiniteStateMachine stateMachine;

    public D_Entity entityData;
    
    public int facingDirection { get; private set; }
    public int lastDamageDirection { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public GameObject aliveGameObject { get; private set; }
    public AnimationToStateMachine animationToStateMachine  { get; private set; }

    

    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private Transform groundCheck;

    private float currentHealth;
    private float currentStunResistance;
    private float lastDamageTime;

    private Vector2 velocityWorkspace;

    protected bool isStunned;
    protected bool isDead;

    public virtual void Start()
    {
        facingDirection = 1;
        currentHealth = entityData.maxHealth;
        currentStunResistance = entityData.stunResistance;
        
        aliveGameObject = transform.Find("Alive").gameObject;
        rb = aliveGameObject.GetComponent<Rigidbody2D>();
        anim = aliveGameObject.GetComponent<Animator>();
        animationToStateMachine = aliveGameObject.GetComponent<AnimationToStateMachine>();

        stateMachine = new FiniteStateMachine();
    }

    public virtual void DamageHop(float velocity)
    {
        velocityWorkspace.Set(rb.velocity.x, velocity);
        rb.velocity = velocityWorkspace;
    }

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = entityData.stunResistance;
    }
    
    public virtual void Damage(AttackDetails attackDetails)
    {
        lastDamageTime = Time.time;
        currentHealth -= attackDetails.damageAmount;
        currentStunResistance -= attackDetails.stunDamageAmount;
        
        DamageHop(entityData.damageHopSpeed);

        Instantiate(entityData.hitParticle, aliveGameObject.transform.position,
            Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        
        if (attackDetails.position.x > aliveGameObject.transform.position.x)
        {
            lastDamageDirection = -1;
        }
        else
        {
            lastDamageDirection = 1;
        }

        if (currentStunResistance <= 0)
        {
            isStunned = true;
        }

        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }
    
    public virtual void Update()
    {
       stateMachine.currentState.LogicUpdate();
       
       anim.SetFloat("yVelocity", rb.velocity.y);

       if (Time.time >= lastDamageTime + entityData.stunRecoverTime)
       {
           ResetStunResistance();
       }
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float velocity)
    {
        velocityWorkspace.Set(facingDirection * velocity, rb.velocity.y);
        rb.velocity = velocityWorkspace;
    }

    public virtual void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        velocityWorkspace.Set(angle.x * velocity * direction, angle.x * velocity);
        rb.velocity = velocityWorkspace;
    }
    
    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, aliveGameObject.transform.right, entityData.wallCheckDistance,entityData.whatIsGround);
    }

    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance,entityData.whatIsGround);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, entityData.groundCheckRadius, entityData.whatIsGround);
    }
    
    public virtual bool CheckPlayerInMinAggroRange()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGameObject.transform.right, entityData.minAggroDistance,
            entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxAggroRange()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGameObject.transform.right, entityData.maxAggroDistance,
            entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGameObject.transform.right,
            entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }
    
    public virtual void Flip()
    {
        facingDirection *= -1;
        aliveGameObject.transform.Rotate(0f, 180f, 0f);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.wallCheckDistance));

        Gizmos.DrawWireSphere((playerCheck.position + (Vector3)(Vector2.right * entityData.closeRangeActionDistance * facingDirection)), 0.2f);
        
        Gizmos.DrawWireSphere((playerCheck.position + (Vector3)(Vector2.right * entityData.maxAggroDistance * facingDirection)), 0.2f);
        Gizmos.DrawWireSphere((playerCheck.position + (Vector3)(Vector2.right * entityData.minAggroDistance * facingDirection)), 0.2f);

    }
}
