using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState
{
    protected D_MeleeAttackState stateData;
    
    public MeleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttackState stateData) : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackPosition.position, stateData.attackRadius, stateData.whatIsPlayer);

        foreach (Collider2D item in detectedObjects)
        {
            IDamageable damageable = item.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.Damage(stateData.attackDamage);
            }

            IKnockbackable knockbackable = item.GetComponent<IKnockbackable>();

            if (knockbackable != null)
            {
                knockbackable.KnockBack(stateData.knockBackAngle, stateData.knockBackStrength, core.Movement.FacingDirection);
            }
        }
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }
}
