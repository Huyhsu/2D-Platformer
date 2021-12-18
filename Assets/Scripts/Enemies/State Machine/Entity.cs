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
    
    public Core Core { get; private set; }
    
    public int lastDamageDirection { get; private set; }
    public Animator anim { get; private set; }
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

    public virtual void Awake()
    {
        Core = GetComponentInChildren<Core>();
        
        currentHealth = entityData.maxHealth;
        currentStunResistance = entityData.stunResistance;
        
        anim = GetComponent<Animator>();
        animationToStateMachine = GetComponent<AnimationToStateMachine>();

        stateMachine = new FiniteStateMachine();
    }

    public virtual void DamageHop(float velocity)
    {
        velocityWorkspace.Set(Core.Movement.RB.velocity.x, velocity);
        Core.Movement.RB.velocity = velocityWorkspace;
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

        Instantiate(entityData.hitParticle, transform.position,
            Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        
        if (attackDetails.position.x > transform.position.x)
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
       
       anim.SetFloat("yVelocity", Core.Movement.RB.velocity.y);

       if (Time.time >= lastDamageTime + entityData.stunRecoverTime)
       {
           ResetStunResistance();
       }
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual bool CheckPlayerInMinAggroRange()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right, entityData.minAggroDistance,
            entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxAggroRange()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right, entityData.maxAggroDistance,
            entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, transform.right,
            entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }

    public virtual void OnDrawGizmos()
    {
        if (Core != null)
        {
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * Core.Movement.FacingDirection * entityData.wallCheckDistance));
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.wallCheckDistance));

            Gizmos.DrawWireSphere((playerCheck.position + (Vector3)(Vector2.right * entityData.closeRangeActionDistance * Core.Movement.FacingDirection)), 0.2f);
            
            Gizmos.DrawWireSphere((playerCheck.position + (Vector3)(Vector2.right * entityData.maxAggroDistance * Core.Movement.FacingDirection)), 0.2f);
            Gizmos.DrawWireSphere((playerCheck.position + (Vector3)(Vector2.right * entityData.minAggroDistance * Core.Movement.FacingDirection)), 0.2f);
        }
        
    }
}
