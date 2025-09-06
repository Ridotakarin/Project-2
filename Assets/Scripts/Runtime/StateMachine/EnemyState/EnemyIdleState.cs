using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyIdleStateData
{
    public float StandStillTime; // Time to stand still before moving

    [Header("Fan Detection Settings")]
    public float fanRange = 5f;
    public float fanAngle = 90f;
    public int fanRayCount = 7;
}

public class EnemyIdleState : IState
{
    private readonly EnemyAI _enemyAI;
    private readonly EnemyIdleStateData _data;
    private float _standStillTimer;
    public EnemyIdleState(EnemyAI enemyAI, EnemyIdleStateData data)
    {
        _enemyAI = enemyAI;
        _data = data;
    }
    public void Enter()
    {
        _standStillTimer = 0;
        _enemyAI.Animator.SetFloat(_enemyAI.SpeedParameter, 0);
    }
    public void StateUpdate()
    {
        bool isDetectedPlayer = _enemyAI.DetectPlayerFan(
            origin: _enemyAI.Rb.position,
            forward: _enemyAI.LastMovement,
            range: _data.fanRange,
            angle: _data.fanAngle,
            rayCount: _data.fanRayCount
        );

        if (isDetectedPlayer)
        {
            // Switch to chasing state, example:
            Debug.Log("Player detected, switch to chasing from idle state!");
            _enemyAI.StateMachine.ChangeState(_enemyAI.ChasingState);
        }


        if (_standStillTimer >= _data.StandStillTime)
        {
            // Transition to the next state (e.g., patrolling or chasing)
            _enemyAI.StateMachine.ChangeState(_enemyAI.PatrollingState);
        }
        else _standStillTimer += Time.deltaTime;
    }
    public void StateFixedUpdate()
    {
        if (_enemyAI.Rb.linearVelocity.magnitude > 0.1f)
        {
            _enemyAI.Rb.AddForce(_enemyAI.Rb.linearVelocity * -_enemyAI.Acceleration, ForceMode2D.Force);
        }
        else
        {
            _enemyAI.Rb.linearVelocity = Vector2.zero;
        }
    }

    public void Exit()
    {
        
    }

    
}
