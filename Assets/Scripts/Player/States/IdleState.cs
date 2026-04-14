using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player, PlayerStateMachine sm) : base(player, sm) { }

    public override void Enter()
    {
        player.anim.Play("Idle");
    }

    public override void LogicUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.S))
        {
            stateMachine.ChangeState(player.crouchState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && player.dashesLeft > 0)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }
        if (player.WantJump() && player.CanUseCoyoteJump())
        {
            player.ConsumeJumpBuffer();
            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (Mathf.Abs(x) > 0.01f)
        {
            stateMachine.ChangeState(player.runState);
            return;
        }
        if (!player.isGrounded)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }
    public override void PhysicsUpdate()
    {
        // no matar la velocidad X, solo dejar que la fricción la reduzca
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.rb.linearVelocity.y);
    }
}