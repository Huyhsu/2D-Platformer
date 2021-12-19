using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : CoreComponent, IDamageable, IKnockbackable
{
    [SerializeField] private float maxKnockBackTime = 0.2f;
    
    private bool isKnockBackActive;
    private float knockBackStartTime;

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        CheckKnockBack();
    }

    public void Damage(float amount)
    {
        Debug.Log(core.transform.parent.name + " Damaged!");
        core.Stats.DecreaseHealth(amount);
    }

    public void KnockBack(Vector2 angle, float strength, int direction)
    {
        core.Movement.SetVelocity(strength, angle, direction);
        core.Movement.CanSetVelocity = false;
        isKnockBackActive = true;
        knockBackStartTime = Time.time;
    }

    private void CheckKnockBack()
    {
        if (isKnockBackActive && core.Movement.CurrentVelocity.y <= 0.01f && core.CollisionSenses.Ground || Time.time >= knockBackStartTime + maxKnockBackTime)
        {
            isKnockBackActive = false;
            core.Movement.CanSetVelocity = true;
        }
    }
}
