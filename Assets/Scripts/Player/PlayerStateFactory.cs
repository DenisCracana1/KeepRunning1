using UnityEngine;

public class PlayerStateFactory
{
    PlayerController player;
    PlayerStateMachine stateMachine;

    public PlayerStateFactory(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }
    public IdleState Idle() => new IdleState(player, stateMachine);
    public RunState Run() => new RunState(player, stateMachine);
    public CrouchState Crouch() => new CrouchState(player, stateMachine);
    public JumpState Jump() => new JumpState(player, stateMachine);
    public FallState Fall() => new FallState(player, stateMachine);
    public DashState Dash() => new DashState(player, stateMachine);
    public WallSlideState WallSlide() => new WallSlideState(player, stateMachine);
    public WallJumpState WallJump() => new WallJumpState(player, stateMachine);
    public ClimbState Climb() => new ClimbState(player, stateMachine);
}