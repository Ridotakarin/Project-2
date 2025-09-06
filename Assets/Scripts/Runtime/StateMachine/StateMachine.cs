using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState _currentState;
    public IState CurrentState
    {
        get { return _currentState; }
        private set { _currentState = value; }
    }

    public void ChangeState(IState newState)
    {
        if (CurrentState == newState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void StateUpdate()
    {
        CurrentState?.StateUpdate();
    }
    public void StateFixedUpdate()
    {
        CurrentState?.StateFixedUpdate();
    }
}
