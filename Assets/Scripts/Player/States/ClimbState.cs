using UnityEngine;

public class ClimbState : PlayerState
{
    bool jumpedThisFrame;

    public ClimbState(PlayerController player, PlayerStateMachine sm) : base(player, sm) { }

    public override void Enter()
    {
        player.anim.Play("climb");
        player.rb.gravityScale = 0f;
        player.rb.linearVelocity = Vector2.zero;
        jumpedThisFrame = false;
    }

    public override void LogicUpdate()
    {
        jumpedThisFrame = false;

        if (!player.isTouchingWall)
        {
            ExitCling();
            stateMachine.ChangeState(player.fallState);
            return;
        }
        if (!Input.GetMouseButton(1))
        {
            ExitCling();
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && player.currentStamina > 0f)
        {
            player.currentStamina -= player.wallClimbStaminaCostJump * Time.deltaTime;
            jumpedThisFrame = true;
            ExitCling();
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }
        if (player.currentStamina <= 0f)
        {
            ExitCling();
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && player.dashesLeft > 0)
        {
            ExitCling();
            stateMachine.ChangeState(player.dashState);
            return;
        }
    }
    public override void PhysicsUpdate()
    {
        float yInput = 0f;

        if (Input.GetKey(KeyCode.W))
            yInput = 1f;
        else if (Input.GetKey(KeyCode.S))
            yInput = -1f;

        float cost = 0f;

        if (Mathf.Abs(yInput) < 0.1f)
        {
            cost = player.wallClimbStaminaCostIdle;
        }
        else
        {
            cost = player.wallClimbStaminaCostMove;
        }

        player.currentStamina -= cost * Time.deltaTime;
        player.currentStamina = Mathf.Max(player.currentStamina, 0f);

        player.rb.linearVelocity = new Vector2(0, yInput * player.climbSpeed);
    }

    public override void Exit()
    {
        ExitCling();
    }

    void ExitCling()
    {
        player.rb.gravityScale = 1f;
        if (player.rb.linearVelocity.y > 0 && !jumpedThisFrame)
        {
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, 0);
        }
    }
}
    