using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchMoveState : PlayerGroundedState
{
    public PlayerCrouchMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.SetBoxCollider2DHeight(playerData.crouchColliderHeight);
    }

    public override void Exit()
    {
        base.Exit();
        
        player.SetBoxCollider2DHeight(playerData.standColliderHeight);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            player.SetVelocityX(playerData.crouchMovementVelocity * xInput);
            player.CheckIfShouldFlip(xInput);
            
            if (xInput == 0)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
            else if (yInput != -1 && !isTouchingCeiling)
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }
    }
}
