using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class NpcController : MonoBehaviour
{
    public float speed = 1f;
    public Vector2 movement;
    public Transform targetPosition;
    public DetectionZone zone;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayableDirector npcDirector;

    private Coroutine choppingRoutine;
    public float detectionRadius = 10f;
    public float raycastSpacing = 2f;
    public LayerMask treeLayer;

    private bool canAttack = true;
    private bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        npcDirector = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (zone != null && zone.detectedColliders.Count > 0 && canAttack)
        {
            animator.Play("Axe_Attack");
        }

        if (targetPosition != null && canMove)
        {
            OnMove();
        }
        if (!canMove)
        {
            OnStop();
        }    
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX * speed, rb.linearVelocityY);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("NpcController destroyed.");
    }

    public void OnMove()
    {
        
    }

    public void OnStop()
    {
        
    }

    private void UpdateAnimation()
    {
        
    }

    public void SetTargetPosition(Transform newTarget)
    {
        targetPosition = newTarget;
    }

    public void StartChoppingTrees()
    {
        if (zone != null && zone.detectedColliders.Count > 0)
        {
            animator.Play("Axe_Attack");
        }
    }

    public void StopChoppingTrees()
    {
        if (choppingRoutine != null)
        {
            StopCoroutine(choppingRoutine);
            choppingRoutine = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector2 origin = transform.position;

        for (float angle = 0; angle < 360; angle += raycastSpacing)
        {
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Gizmos.color = Color.green;
            Gizmos.DrawRay(origin, dir * detectionRadius);

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, detectionRadius, treeLayer);

            if (hit.collider != null && hit.collider.CompareTag("Tree"))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(origin, dir * detectionRadius);

                if (targetPosition == null)
                {
                    targetPosition = hit.collider.transform;
                }
            }
        }
    }

    public void StopAllAction()
    {
        canAttack = false;
        canMove = false;
        StopChoppingTrees();
    }

    public void StartAllAction()
    {
        canAttack = true;
        canMove = true;
    }
}
