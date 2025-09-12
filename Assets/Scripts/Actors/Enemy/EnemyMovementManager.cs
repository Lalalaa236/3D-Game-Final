using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovementManager : ActorMovementManager
{
    private EnemyManager enemy;
    private NavMeshAgent agent;

    [Header("Move")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("NavMesh")]
    [SerializeField] private float maxNavClampDistance = 0.5f;
    [SerializeField] private float raycastPadding = 0.05f;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyManager>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void Idle()
    {
        enemy.animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
        enemy.animator.SetFloat("Vertical",   0f, 0.1f, Time.deltaTime);
    }

    public void ChaseTarget(Transform target)
    {
        if (!enemy.canMove || target == null) return;

        Vector3 toTarget = (target.position - transform.position);
        toTarget.y = 0f;
        float dist = toTarget.magnitude;

        if (dist > 0.01f) toTarget /= dist;

        // animator params for blend tree
        enemy.animator.SetFloat("Horizontal", Vector3.Dot(toTarget, transform.right),  0.1f, Time.deltaTime);
        enemy.animator.SetFloat("Vertical",   Vector3.Dot(toTarget, transform.forward),0.1f, Time.deltaTime);

        // desired horizontal step
        Vector3 desired = toTarget * walkSpeed * Time.deltaTime;
        Vector3 allowed = ComputeNavSafeDelta(desired);

        Vector3 gravity = enemy.GetGravityDelta();
        enemy.characterController.Move(allowed + gravity);

        RotateTowards(target.position);

        PostSnapToNavMesh();
    }

    public void BackOffStep(float backSpeed)
    {
        Vector3 dir = -transform.forward;
        Vector3 desired = dir * backSpeed * Time.deltaTime;
        Vector3 allowed = ComputeNavSafeDelta(desired);

        Vector3 gravity = enemy.GetGravityDelta();
        enemy.characterController.Move(allowed + gravity);

        PostSnapToNavMesh();
    }

    public void RotateTowards(Vector3 worldPos)
    {
        Vector3 dir = worldPos - transform.position; dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
    }

    // ── nav helpers (copied from Player, trimmed) ────────────────────────────────
    private Vector3 ComputeNavSafeDelta(Vector3 desiredHorizontalDelta)
    {
        Vector3 origin = transform.position;
        Vector3 desired = origin + desiredHorizontalDelta;
        Vector3 allowed = Vector3.zero;

        bool haveFrom = NavMesh.SamplePosition(origin, out var fromHit, maxNavClampDistance, NavMesh.AllAreas);
        bool haveTo   = NavMesh.SamplePosition(desired, out var toHit,   maxNavClampDistance, NavMesh.AllAreas);

        if (haveFrom && haveTo)
        {
            if (NavMesh.Raycast(fromHit.position + Vector3.up * raycastPadding,
                                toHit.position   + Vector3.up * raycastPadding,
                                out var rayHit, NavMesh.AllAreas))
            {
                Vector3 attempted = toHit.position - fromHit.position;
                Vector3 slide = Vector3.ProjectOnPlane(attempted, rayHit.normal);
                allowed = slide;
            }
            else allowed = toHit.position - fromHit.position;
        }
        else
        {
            if (NavMesh.SamplePosition(desired, out var clampHit, maxNavClampDistance, NavMesh.AllAreas))
                allowed = clampHit.position - origin;
            else allowed = Vector3.zero;
        }
        return allowed;
    }

    private void PostSnapToNavMesh()
    {
        if (agent && NavMesh.SamplePosition(transform.position, out var postHit, maxNavClampDistance, NavMesh.AllAreas))
        {
            transform.position = postHit.position;
            agent.nextPosition = postHit.position;
        }
    }
}
