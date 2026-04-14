using UnityEngine;

public class CrouchState : PlayerState
{
    public CrouchState(PlayerController player, PlayerStateMachine sm) : base(player, sm) { }

    public override void Enter()
    {
        player.anim.Play("crounch");
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
    }
    public override void LogicUpdate()
    {
        if (player.WantJump() && player.isGrounded)
        {
            player.ConsumeJumpBuffer();
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && player.dashesLeft > 0)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }

        if (!Input.GetKey(KeyCode.S))
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }
    public override void PhysicsUpdate()
    {
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
    }
}