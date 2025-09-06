using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : NetworkBehaviour
{
    #region StateMachine Setup
    public StateMachine StateMachine { get; private set; }

    // State Data
    public EnemyIdleStateData IdleStateData;
    public EnemyPatrollingStateData PatrollingStateData;
    public EnemyChasingStateData ChasingStateData;

    // State References
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrollingState PatrollingState { get; private set; }
    public EnemyChasingState ChasingState { get; private set; }
    #endregion

    #region Components
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public Damageable Damageable;
    [HideInInspector] public Collider2D Collider;
    #endregion

    #region Variables
    [SerializeField]
    private Item[] _itemsToDrop;

    [SerializeField]
    private int[] _itemDropNum;
    [SerializeField]
    private float[] _itemDropNumRatios;

    [SerializeField]
    private GameObject itemDropPrefab;

    public string playerTag = "Player";
    public LayerMask playerLayer;
    public LayerMask patrollingObstacleLayer;
    public LayerMask visionCheckableLayer;

    // Current target player transform (null if none)
    public Transform TargetPlayer { get; private set; }

    // For gizmo visualization
    public Vector2 DetectorOriginOffset; // Offset for the detection origin
    [HideInInspector] public Vector2 GizmoFanOrigin;
    [HideInInspector] public Vector2 GizmoFanDirection;
    [HideInInspector] public float GizmoFanRange;
    [HideInInspector] public float GizmoFanAngle;
    [HideInInspector] public int GizmoFanRayCount;

    public float Acceleration;

    [SerializeField]
    private bool _canMove = true;
    public bool CanMove
    {
        get {  return _canMove; }
        set 
        { 
            _canMove = value;
            if (!_canMove)
                Rb.linearVelocity = Vector2.zero;
        }
    }
    
    public bool CanAttack = true; // Flag to control attacking
    
    #endregion

    #region Projectile Setting
    [SerializeField] private GameObject projectilePrefab; 
    [SerializeField] private float projectileSpeed; 
    [SerializeField] private float _projectileDamage;
    #endregion

    #region Animations Variables
    public string HorizontalParameter 
    {
        get => "Horizontal";
    }
        
    public string VerticalParameter 
    {
        get => "Vertical";
    }
    public string SpeedParameter
    {
        get => "Speed";
    }

    public string AttackParameter
    {
        get => "Attack";
    }

    public string HurtParameter
    {
        get => "Hurt";
    }
    private Vector2 _lastMovement;
    public Vector2 LastMovement
    {
        get => _lastMovement;
        set
        {
            _lastMovement = value;
            // Clamp or round to -1, 0, or 1 to snap to a single direction
            float clampedX = Mathf.Abs(Mathf.Round(_lastMovement.x));
            float clampedY = Mathf.Round(_lastMovement.y);

            Animator.SetFloat(HorizontalParameter, clampedX);
            Animator.SetFloat(VerticalParameter, clampedY);
            if (_lastMovement.x > 0 && !IsFacingRight) 
            {
                IsFacingRight = true;
            }
            else if(_lastMovement.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }
    }
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set
        {
            _isFacingRight = value;
            transform.localScale = _isFacingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }
    }
    #endregion

    #region Events
    #endregion
    private void Awake()
    {

        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Damageable = GetComponent<Damageable>();
        Collider = GetComponent<Collider2D>();

        StateMachine = new StateMachine();

        IdleState = new EnemyIdleState(this, IdleStateData);
        PatrollingState = new EnemyPatrollingState(this, PatrollingStateData);
        ChasingState = new EnemyChasingState(this, ChasingStateData);
    }

    private void OnCanMoveChanged(bool previousValue, bool newValue)
    {
        Debug.Log("run on can moved changed");
        if (!newValue) Rb.linearVelocity = Vector2.zero;
    }
    private void Start()
    {
        StateMachine.ChangeState(IdleState);
    }
    private void Update()
    {
        StateMachine.StateUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.StateFixedUpdate();
    }

    public void ApplyMovement(Vector2 direction, float speed, float acceleration)
    {
        if (!CanMove)
        {
            if (Rb.linearVelocity.magnitude > 0.1f)
                Rb.AddForce(Rb.linearVelocity * -acceleration, ForceMode2D.Force);
            else
                Rb.linearVelocity = Vector2.zero;
            return;
        }

        if (direction != Vector2.zero)
        {
            Rb.AddForce(direction * acceleration, ForceMode2D.Force);

            if (Rb.linearVelocity.magnitude > speed)
                Rb.linearVelocity = Rb.linearVelocity.normalized * speed;

            LastMovement = direction;
        }
        else
        {
            if (Rb.linearVelocity.magnitude > 0.1f)
                Rb.AddForce(Rb.linearVelocity * -acceleration, ForceMode2D.Force);
            else
                Rb.linearVelocity = Vector2.zero;
        }
    }


    public bool DetectPlayerFan(Vector2 origin, Vector2 forward, float range, float angle, int rayCount)
    {
        origin += DetectorOriginOffset;
        GizmoFanOrigin = origin;
        GizmoFanDirection = forward;
        GizmoFanRange = range;
        GizmoFanAngle = angle;
        GizmoFanRayCount = rayCount;

        float halfAngle = angle / 2f;
        float angleStep = angle / (rayCount - 1);
        float startAngle = -halfAngle;

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, currentAngle) * forward;

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, range, visionCheckableLayer | playerLayer);

            if (hit.collider != null && hit.collider.CompareTag(playerTag))
            {
                SetTargetPlayer(hit.collider.transform);
                return true;
            }
        }
        return false;
    }

    public void SetTargetPlayer(Transform player)
    {
        if (TargetPlayer != null) return;
        TargetPlayer = player;
    }

    public void ClearTargetPlayer()
    {
        TargetPlayer = null;
    }

    public void AttackTriggerByAnimationEvent()
    {

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        if (projectile.GetComponent<ArrowProjectile>() != null)
        {
            Vector2 AttackDirection = ((Vector2)TargetPlayer.position - Rb.position).normalized;
            projectile.GetComponent<ArrowProjectile>().DamageAmount = _projectileDamage;
            // Rotate projectile to face the direction
            float angle = Mathf.Atan2(AttackDirection.y, AttackDirection.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

            // Optionally, set velocity
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.linearVelocity = AttackDirection.normalized * projectileSpeed;
        }
        else if (projectile.GetComponentInChildren<ExplosionProjectile>() != null)
        {
            FakeHeightObject fakeHeightObject = projectile.GetComponentInChildren<FakeHeightObject>();
            fakeHeightObject.Initialize(TargetPlayer.position);
            ExplosionProjectile explosionProjectile = fakeHeightObject.GetComponent<ExplosionProjectile>();
            explosionProjectile.DamageAmount = _projectileDamage;
        }

    }


    public void StopAllAction()
    {
        CanMove = false;
        CanAttack = false;
    }

    public void StartAllAction()
    {
        CanMove = true;
        CanAttack = true;
    }

    public void EnemyOnHit(Vector2 knockBackDirection)
    {
        if(!Damageable.IsAlive) Animator.SetTrigger("Dead");
        else
        {
            Animator.SetTrigger(HurtParameter);
            StateMachine.ChangeState(IdleState);
            LastMovement = -knockBackDirection.normalized;
            AudioManager.Instance.PlaySFX("goblin_hurt");
            StartCoroutine(ApplyKnockback(knockBackDirection));
        }
    }

    private IEnumerator ApplyKnockback(Vector2 knockBackDirection)
    {
        yield return new WaitUntil(() => CanMove == false);

        Rb.AddForce(knockBackDirection, ForceMode2D.Impulse);
        Debug.Log("knockBack apply: " + knockBackDirection);
    }

    public IEnumerator DestroyAfter(float delay)
    {
        if (!IsServer) yield break;
        yield return new WaitForSeconds(delay);
        DropItem();
        GetComponent<NetworkObject>().Despawn(true);
    }
    public void DropItem()
    {
        int numItem = 0;
        numItem = UtilsClass.PickOneByRatio(_itemDropNum, _itemDropNumRatios);
        Item itemGotPicked = _itemsToDrop[Random.Range(0, _itemsToDrop.Length)];
        ItemWorld itemWorldDropInfo = new ItemWorld(System.Guid.NewGuid().ToString(), itemGotPicked, numItem, transform.position, 1);
        ItemWorldManager.Instance.DropItemIntoWorld(itemWorldDropInfo, false, false);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(StateMachine.CurrentState == PatrollingState && collision.gameObject.layer == patrollingObstacleLayer)
    //    {
    //        StateMachine.ChangeState(IdleState);
    //    }
    //}


    // Draw the fan rays in the editor for debugging

    private void OnDrawGizmos()
    {
        if (GizmoFanRayCount <= 1 || GizmoFanRange <= 0f)
            return;

        float halfAngle = GizmoFanAngle / 2f;
        float angleStep = GizmoFanAngle / (GizmoFanRayCount - 1);
        float startAngle = -halfAngle;

        for (int i = 0; i < GizmoFanRayCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, currentAngle) * GizmoFanDirection;

            // Perform a raycast to check for obstacle or player
            RaycastHit2D hit = Physics2D.Raycast(GizmoFanOrigin, dir, GizmoFanRange, visionCheckableLayer | playerLayer);

            if (hit.collider != null)
            {
                // Change color if it hits a player
                if (hit.collider.CompareTag(playerTag))
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.gray;

                Gizmos.DrawRay(GizmoFanOrigin, dir * hit.distance);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(GizmoFanOrigin, dir * GizmoFanRange);
            }
        }

        
        if (StateMachine?.CurrentState == PatrollingState)
        {
            Gizmos.color = PatrollingState.RayHit ? Color.red : Color.cyan;
            Vector2 origin = PatrollingState.RayOrigin;
            Vector2 dir = PatrollingState.RayDirection.normalized;
            float length = PatrollingState.RayLength;
            Gizmos.DrawRay(origin, dir * length);
        }else if(StateMachine?.CurrentState == ChasingState)
        {
            Vector2 origin = ChasingState.RayOrigin + ChasingStateData.rayOriginOffset;

            for (int i = 0; i < ChasingState.RayDirections.Length; i++)
            {
                Vector2 dir = ChasingState.RayDirections[i];
                bool hit = ChasingState.RayHits[i];

                Gizmos.color = hit ? Color.red : Color.cyan;
                Gizmos.DrawRay(origin, dir * ChasingStateData.checkRadius);
            }
        }
    }

}
