using UnityEngine;

public class WallSlideState : PlayerState
{
    public WallSlideState(PlayerController player, PlayerStateMachine sm) : base(player, sm) { }
    public override void Enter()
    {
        player.anim.Play("wall");
    }

    public override void LogicUpdate()
    {
        if (!player.isTouchingWall)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (player.WantJump())
        {
            player.ConsumeJumpBuffer();
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }
        if (Input.GetMouseButton(1) && player.currentStamina > 0f)
        {
            stateMachine.ChangeState(player.climbState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && player.dashesLeft > 0)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }
    }
    public override void PhysicsUpdate()
    {
        float y = Mathf.Max(player.rb.linearVelocity.y, player.wallSlideSpeed);
        player.rb.linearVelocity = new Vector2(0, y);
    }
}
