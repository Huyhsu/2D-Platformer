using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : CoreComponent, IDamageable, IKnockbackable
{
    private bool isKnockBackActive;
    private float knockBackStartTime;

    public void LogicUpdate()
    {
        CheckKnockBack();
    }
    
    public void Damage(float amount)
    {
        Debug.Log(core.transform.parent.name + " Damaged!");
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
        if (isKnockBackActive && core.Movement.CurrentVelocity.y <= 0.01f && core.CollisionSenses.Ground)
        {
            isKnockBackActive = false;
            core.Movement.CanSetVelocity = true;
        }
    }
}
