using System;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[Serializable]
public class EnemyPatrollingStateData
{
    public float walkSpeed;
    public float patrolRadius = 5f;
    public float arrivalThreshold = 0.2f;

    // For obstacle detection
    public float rayOffsetLength = 0.2f;

    [Header("Fan Detection Settings")]
    public float fanRange = 5f;
    public float fanAngle = 90f;
    public int fanRayCount = 7;
}

public class EnemyPatrollingState : IState
{
    private readonly EnemyAI _enemyAI;
    private readonly EnemyPatrollingStateData _data;
    private Vector2 _targetPosition;
    private bool _hasTarget;

    // For check obstacles
    private Vector2 _rayOrigin;
    private Vector2 _rayDirection;
    private float _rayLength;
    private bool _rayHit;

    public Vector2 RayOrigin => _rayOrigin;
    public Vector2 RayDirection => _rayDirection;
    public float RayLength => _rayLength;
    public bool RayHit => _rayHit;

    public EnemyPatrollingState(EnemyAI enemyAI, EnemyPatrollingStateData data)
    {
        _enemyAI = enemyAI;
        _data = data;
    }

    public void Enter()
    {
        PickNewPatrolPoint();
        _enemyAI.Animator.SetFloat(_enemyAI.SpeedParameter, _data.walkSpeed);
    }

    public void StateUpdate()
    {
        if (!_enemyAI.CanMove) return;
        DetectPlayerHandler();
        
    }

    public void StateFixedUpdate()
    {
        if (!_hasTarget || !_enemyAI.CanMove) return;
        PatrollingHandler();
        
    }



    public void Exit()
    {
        _hasTarget = false;
    }
    private void PickNewPatrolPoint()
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * _data.patrolRadius;
        Vector2 basePosition = _enemyAI.Rb.position; // ← use Rb.position for consistency
        _targetPosition = basePosition + randomPoint;
        _hasTarget = true;
    }

    private void DetectPlayerHandler()
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
            _enemyAI.StateMachine.ChangeState(_enemyAI.ChasingState);
        }
    }

    private void PatrollingHandler()
    {
        Vector2 currentPos = _enemyAI.Rb.position;
        Vector2 direction = (_targetPosition - currentPos).normalized;

        // Fan-shaped obstacle detection
        float rayLength = 0.7f;
        float halfAngle = _data.fanAngle / 4f;
        int rayCount = 3;

        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount == 1 ? 0.5f : (float)i / (rayCount - 1); // Normalize between 0 and 1
            float angleOffset = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector2 rayDir = Quaternion.Euler(0, 0, angleOffset) * direction;

            RaycastHit2D hit = Physics2D.Raycast(currentPos, rayDir, rayLength, _enemyAI.patrollingObstacleLayer);

            // Optional: draw rays in editor
            Debug.DrawRay(currentPos, rayDir * rayLength, hit.collider ? Color.red : Color.green);

            if (hit.collider != null)
            {
                Debug.Log("Obstacle detected in fan: " + hit.collider.name);
                _enemyAI.StateMachine.ChangeState(_enemyAI.IdleState);
                return;
            }
        }

        _enemyAI.ApplyMovement(direction, _data.walkSpeed, _enemyAI.Acceleration);

        if (Vector2.Distance(currentPos, _targetPosition) <= _data.arrivalThreshold)
        {
            _enemyAI.StateMachine.ChangeState(_enemyAI.IdleState);
        }
    }

}
