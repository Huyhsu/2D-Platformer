using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    private Weapon _weapon;

    private int xInput;
    
    private float velocityToSet;
    
    private bool setVelocity;
    private bool shouldCheckFlip;
    
    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        setVelocity = false;
        
        _weapon.EnterWeapon();
    }

    public override void Exit()
    {
        base.Exit();
        
        _weapon.ExitWeapon();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormalizedInputX;

        if (shouldCheckFlip)
        {
            player.CheckIfShouldFlip(xInput);
        }
        
        if (setVelocity)
        {
            player.SetVelocityX(velocityToSet * player.FacingDirection);
        }
    }

    public void SetWeapon(Weapon weapon)
    {
        this._weapon = weapon;
        this._weapon.InitializeWeapon(this);
    }

    public void SetPlayerVelocity(float velocity)
    {
        player.SetVelocityX(velocity * player.FacingDirection);

        velocityToSet = velocity;
        setVelocity = true;
    }

    public void SetFlipCheck(bool value)
    {
        shouldCheckFlip = value;
    }
    
    #region Animation Triggers

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        isAbilityDone = true;
    }

    #endregion
}