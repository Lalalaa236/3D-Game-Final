using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AITarget : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    public void SetTarget(Transform t) => target = t;

    [Header("Distances")]
    [SerializeField] private float aggroRadius = 10f;
    [SerializeField] private float loseAggroRadius = 15f;
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float stoppingBuffer = 0.2f;

    [Header("Repathing")]
    [SerializeField] private float repathInterval = 0.2f;
    [SerializeField] private float destRefreshThreshold = 0.3f;
    [Header("Animation")]
    [SerializeField] private float dampTime = 0.12f;
    [SerializeField] private float animSmoothing = 0.12f;
    [SerializeField] private float forwardDeadzone = 0.03f;
    [SerializeField] private bool allowBackpedal = false;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 3.0f;
    private float attackTimer = 0f;

    [Header("Rotation")]
    [SerializeField] private bool rotateToVelocity = true;
    [SerializeField] private float rotateSpeed = 12f;

    private NavMeshAgent agent;
    private Animator animator;

    private bool hasAggro = false;
    private float sqrAggro, sqrLose, sqrAttack;
    private float repathTimer = 0f;
    private Vector3 lastDestSent = Vector3.positiveInfinity;

    private Vector2 hvSmooth = Vector2.zero;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!target) Debug.LogError("AITarget: No target assigned.");

        agent.updatePosition = true;
        agent.updateRotation = false;
        animator.applyRootMotion = false;

        agent.autoBraking = false;

        agent.stoppingDistance = Mathf.Max(0f, attackRange - stoppingBuffer);

        sqrAggro = aggroRadius * aggroRadius;
        sqrLose = loseAggroRadius * loseAggroRadius;
        sqrAttack = attackRange * attackRange;
        if (target == null) TryResolveTarget();
    }

    void Update()
    {
        if (!target) return;

        if (attackTimer > 0f) attackTimer -= Time.deltaTime;

        Vector3 toTarget = target.position - transform.position; toTarget.y = 0f;
        float sqrDist = toTarget.sqrMagnitude;

        if (!hasAggro) { if (sqrDist <= sqrAggro) hasAggro = true; }
        else { if (sqrDist >= sqrLose) hasAggro = false; }

        bool inAttack = (sqrDist <= sqrAttack) && hasAggro;

        if (inAttack)
        {
            agent.isStopped = true;
            Face(target.position);

            if (attackTimer <= 0f)
            {
                animator.SetBool("isAttack", true);
                animator.SetFloat("Horizontal", 0f, dampTime, Time.deltaTime);
                animator.SetFloat("Vertical", 0f, dampTime, Time.deltaTime);
                attackTimer = attackCooldown;
            }
            else
            {
                animator.SetBool("isAttack", false);
                animator.SetFloat("Horizontal", 0f, dampTime, Time.deltaTime);
                animator.SetFloat("Vertical", 0f, dampTime, Time.deltaTime);
            }
        }
        else if (hasAggro)
        {
            animator.SetBool("isAttack", false);

            agent.isStopped = false;

            repathTimer -= Time.deltaTime;
            if (repathTimer <= 0f)
            {
                if ((lastDestSent - target.position).sqrMagnitude >= destRefreshThreshold * destRefreshThreshold)
                {
                    agent.SetDestination(target.position);
                    lastDestSent = target.position;
                }
                repathTimer = repathInterval;
            }

            UpdateAnimParamsFromVelocity();

            if (rotateToVelocity)
                RotateToVelocity();
        }
        else
        {
            animator.SetBool("isAttack", false);
            agent.isStopped = true;

            animator.SetFloat("Horizontal", 0f, dampTime, Time.deltaTime);
            animator.SetFloat("Vertical", 0f, dampTime, Time.deltaTime);
        }
    }

    private void UpdateAnimParamsFromVelocity()
    {
        Vector3 vel = agent.velocity;
        vel.y = 0f;

        float speedNorm = vel.magnitude / Mathf.Max(agent.speed, 0.001f);
        Vector3 local = transform.InverseTransformDirection(vel.normalized * speedNorm);

        float vRaw = local.z;
        float hRaw = local.x;

        if (Mathf.Abs(vRaw) < forwardDeadzone) vRaw = 0f;
        if (!allowBackpedal && vRaw < 0f) vRaw = 0f;

        float t = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.0001f, animSmoothing));
        hvSmooth = Vector2.Lerp(hvSmooth, new Vector2(hRaw, vRaw), t);

        animator.SetFloat("Horizontal", Mathf.Clamp(hvSmooth.x, -1f, 1f), dampTime, Time.deltaTime);
        animator.SetFloat("Vertical", Mathf.Clamp(hvSmooth.y, 0f, 1f), dampTime, Time.deltaTime);
    }

    private void RotateToVelocity()
    {
        Vector3 v = agent.velocity; v.y = 0f;
        if (v.sqrMagnitude > 0.0004f)
        {
            Quaternion look = Quaternion.LookRotation(v);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
        }
    }

    private void Face(Vector3 lookAtPos)
    {
        Vector3 dir = lookAtPos - transform.position; dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);
        }
    }
    
    private void TryResolveTarget()
    {
        var go = GameObject.FindWithTag("Player");
        if (go != null) target = go.transform;
        else Debug.LogWarning($"{name}: No target set and no GameObject tagged 'Player' found.");
    }
}
