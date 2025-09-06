using System;
using UnityEngine;

[Serializable]
public class EnemyChasingStateData
{
    public float chaseSpeed;

    [Header("Fan Detection Settings")]
    public float fanRange = 5f;
    public float fanAngle = 90f;
    public int fanRayCount = 7;

    [Header("Surround Check Settings")]
    public float checkRadius;
    public int rayCount;
    public Vector2 rayOriginOffset;
    public float repulsionStrength;

    public float TimeToForgetPlayer;
    public float AttackCooldown;
    public float AttackRange;
}

public class EnemyChasingState : IState
{
    private readonly EnemyAI _enemyAI;
    private readonly EnemyChasingStateData _data;

    private Vector2[] _rayDirections;
    private bool[] _rayHits;

    public Vector2[] RayDirections => _rayDirections;
    public bool[] RayHits => _rayHits;
    public Vector2 RayOrigin => _enemyAI.Rb.position;

    private float _forgetPlayerCounter;
    private float _attackCooldownCounter;
    private bool _isDetectedPlayer = false;
    public EnemyChasingState(EnemyAI enemyAI, EnemyChasingStateData data)
    {
        _enemyAI = enemyAI;
        _data = data;
        _rayDirections = new Vector2[_data.rayCount];
        _rayHits = new bool[_data.rayCount];
    }

    public void Enter()
    {
        _forgetPlayerCounter = 0f;
        _attackCooldownCounter = _data.AttackCooldown;
        _enemyAI.Animator.SetFloat(_enemyAI.SpeedParameter, _data.chaseSpeed);
    }

    public void StateUpdate()
    {
        if (!_enemyAI.CanMove) return;
        DetectPlayerHandler();
        AttackHandler();
    }

    public void StateFixedUpdate()
    {
        if (_enemyAI.TargetPlayer == null || !_enemyAI.CanMove) return;
        ChasingHandler();

    }




    public void Exit()
    {
        Debug.Log("Exit chasing state");
        _isDetectedPlayer = false;
    }

    private void DetectPlayerHandler()
    {
        _isDetectedPlayer = _enemyAI.DetectPlayerFan(
            origin: _enemyAI.Rb.position,
            forward: _enemyAI.LastMovement,
            range: _data.fanRange,
            angle: _data.fanAngle,
            rayCount: _data.fanRayCount
        );

        if (!_isDetectedPlayer) _forgetPlayerCounter += Time.deltaTime;
        else _forgetPlayerCounter = 0;
        if (_forgetPlayerCounter >= _data.TimeToForgetPlayer)
        {
            _enemyAI.ClearTargetPlayer();
            _enemyAI.StateMachine.ChangeState(_enemyAI.IdleState);
        }
    }

    private void AttackHandler()
    {
        if(_attackCooldownCounter >= _data.AttackCooldown)
        {
            if(_enemyAI.TargetPlayer == null || 
                Vector2.Distance(_enemyAI.TargetPlayer.position, _enemyAI.Rb.position) > _data.AttackRange ||
                !_isDetectedPlayer)
                return;
            _enemyAI.Animator.SetTrigger(_enemyAI.AttackParameter);
            _attackCooldownCounter = 0f; // Reset cooldown after attack
        }
        else
        {
            _attackCooldownCounter += Time.deltaTime; // Increment cooldown timer
        }
    }

    private void ChasingHandler()
    {
        Vector2 origin = _enemyAI.Rb.position;
        Vector2 targetPosition = _enemyAI.TargetPlayer.position;
        Vector2 toPlayerDir = (targetPosition - origin).normalized;

        float angleStep = 360f / _data.rayCount;
        float bestDot = -1f;
        Vector2 bestDirection = Vector2.zero;
        Vector2 repulsionVector = Vector2.zero;
        bool anyObstacleDetected = false;

        for (int i = 0; i < _data.rayCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            _rayDirections[i] = dir;

            RaycastHit2D hit = Physics2D.Raycast(origin + _data.rayOriginOffset, dir, _data.checkRadius, _enemyAI.patrollingObstacleLayer);
            _rayHits[i] = hit.collider != null;

            if (_rayHits[i])
            {
                anyObstacleDetected = true;
                float distanceFactor = 1f - (hit.distance / _data.checkRadius);
                repulsionVector -= dir * distanceFactor;
            }
            else
            {
                float dot = Vector2.Dot(toPlayerDir, dir);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDirection = dir;
                }
            }
        }

        Vector2 moveDir;

        if (!anyObstacleDetected)
        {
            moveDir = toPlayerDir;
        }
        else if (bestDot > 0f)
        {
            moveDir = bestDirection;
        }
        else
        {
            // completely blocked, fallback: repulsion only
            moveDir = -repulsionVector.normalized;
        }

        // Apply main movement logic (used for velocity + facing direction)
        _enemyAI.ApplyMovement(moveDir, _data.chaseSpeed, acceleration: _enemyAI.Acceleration);

        // Apply repulsion force without affecting movement direction (optional tuning factor)
        if (repulsionVector != Vector2.zero)
        {
            Vector2 repulsionForce = repulsionVector.normalized * _data.repulsionStrength;
            _enemyAI.Rb.AddForce(repulsionForce, ForceMode2D.Force);
        }
    }



}

