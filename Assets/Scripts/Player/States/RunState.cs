using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerController player, PlayerStateMachine sm) : base(player, sm) { }
    public override void Enter()
    {
        player.anim.Play("run");
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
        if (!player.isGrounded)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
        if (Mathf.Abs(x) < 0.01f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }
    public override void PhysicsUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        player.CheckFlip(x);

        float target = x * player.maxRunSpeed;
        float newX = Mathf.MoveTowards(player.rb.linearVelocity.x, target, player.moveAccel * Time.fixedDeltaTime);

        player.rb.linearVelocity = new Vector2(newX, player.rb.linearVelocity.y);

        if (Mathf.Abs(player.rb.linearVelocity.x) > player.maxRunSpeed)
        {
            float decayed = Mathf.Lerp(
                player.rb.linearVelocity.x,
                Mathf.Sign(player.rb.linearVelocity.x) * player.maxRunSpeed,
                player.speedDecay * Time.fixedDeltaTime
            );

            player.rb.linearVelocity = new Vector2(decayed, player.rb.linearVelocity.y);
        }
    }
}