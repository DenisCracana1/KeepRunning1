using UnityEngine;

public class WallJumpState : PlayerState
{
    public WallJumpState(PlayerController player, PlayerStateMachine sm) : base(player, sm) { }

    public override void Enter()
    {
        int dir = -player.wallDir;

        player.rb.linearVelocity = new Vector2(
            dir * player.wallJumpHorizontalForce,
            player.wallJumpVerticalForce
        );
    }
    public override void LogicUpdate()
    {
        if (player.rb.linearVelocity.y <= 0)
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