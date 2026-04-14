using UnityEngine;

public class FallState : PlayerState
{
    public FallState(PlayerController player, PlayerStateMachine sm) : base(player, sm) { }

    public override void Enter()
    {
        player.anim.Play("fall");
    }

    public override void LogicUpdate()
    {
        var info = player.anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("jump") && info.normalizedTime < 1f)
            return;

        if (player.isGrounded)
        {
            if (Mathf.Abs(player.rb.linearVelocity.x) > 0.1f)
                stateMachine.ChangeState(player.runState);
            else
                stateMachine.ChangeState(player.idleState);
            return;
        }

        if (player.WantJump() && player.CanUseCoyoteJump())
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

        if (player.isTouchingWall && Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
        {
            stateMachine.ChangeState(player.wallSlideState);
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
