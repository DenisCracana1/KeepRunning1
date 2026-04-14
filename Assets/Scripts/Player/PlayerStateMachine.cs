using UnityEngine;
public class PlayerStateMachine
{
    public PlayerState currentState;

    public void Initialize(PlayerState startState)
    {
        currentState = startState;
        currentState.Enter();
    }
    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}