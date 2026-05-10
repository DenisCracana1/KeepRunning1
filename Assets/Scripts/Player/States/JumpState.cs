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
        // 1. Prioritat absoluta: Si toquem terra, anem a Idle o Run
        if (player.isGrounded)
        {
            if (Mathf.Abs(player.rb.linearVelocity.x) > 0.1f)
                stateMachine.ChangeState(player.runState);
            else
                stateMachine.ChangeState(player.idleState);
            return;
        }

        // 2. Dash: Ara es pot fer encara que l'animació de salt no hagi acabat
        if (Input.GetKeyDown(KeyCode.LeftShift) && player.dashesLeft > 0)
        {
            stateMachine.ChangeState(player.dashState);
            return;
        }

        // 3. Transició a caiguda: Quan la velocitat vertical sigui negativa
        if (player.rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.fallState);
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
