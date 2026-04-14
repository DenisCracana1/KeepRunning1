using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerController player, PlayerStateMachine sm) : base(player, sm) { }

    public override void Enter()
    {
        player.anim.Play("jump");
        bool bunny = Mathf.Abs(player.rb.linearVelocity.x) > player.maxRunSpeed * 0.9f;
        float mult = bunny ? player.bunnyHopMultiplier : 1f;

        player.rb.linearVelocity = new Vector2(
            player.rb.linearVelocity.x,
            player.jumpForce * mult
        );

        player.ClearJumpIntent();
    }

    public override void LogicUpdate()
    {
        if (player.isGrounded)
        {
            if (Mathf.Abs(player.rb.linearVelocity.x) > 0.1f)
                stateMachine.ChangeState(player.runState);
            else
                stateMachine.ChangeState(player.idleState);
            return;
        }

        var info = player.anim.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime < 1f)
            return;

        if (player.rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
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
