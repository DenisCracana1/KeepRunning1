using UnityEngine;

public class DashState : PlayerState
{
    float timer;
    Vector2 dashDir;

    public DashState(PlayerController player, PlayerStateMachine sm) : base(player, sm) { }

    public override void Enter()
    {
        player.dashesLeft--;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        dashDir = new Vector2(x, y);
        if (dashDir == Vector2.zero)
            dashDir = new Vector2(Mathf.Sign(player.transform.localScale.x), 0);

        dashDir = dashDir.normalized;

        player.isHyperDash = (dashDir.y < 0 && Mathf.Abs(dashDir.x) > 0.1f);

        player.rb.linearVelocity = dashDir * player.dashForce;

        timer = player.dashDuration;
    }
    public override void LogicUpdate()
    {
        timer -= Time.deltaTime;

        if (player.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            if (player.isHyperDash)
            {
                player.rb.linearVelocity = new Vector2(
                    player.rb.linearVelocity.x,
                    player.waveDashVerticalBoost
                );
            }

            stateMachine.ChangeState(player.jumpState);
            return;
        }
        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
        if (timer <= 0)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }
    }
}