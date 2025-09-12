using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : ActorManager
{
    [HideInInspector] public EnemyMovementManager movement;
    [HideInInspector] public EnemyAnimatorManager anim;
    [HideInInspector] public EnemyStatsManager stats;
    [HideInInspector] public NavMeshAgent agent;

    [Header("Target")]
    public PlayerManager target;

    [Header("AI Radii")]
    public float aggroRadius = 12f;
    public float loseAggroRadius = 18f;
    public float attackRange = 2.3f;

    [Header("Attack")]
    public string attackAnim = "Enemy_Attack_01";
    public float attackCooldown = 2.0f;
    public float backOffDistance = 1.5f;
    public float backOffDuration = 0.30f;

    [Header("Gravity")]
    public float gravity = -20f;
    public float groundedStick = -2f;
    [HideInInspector] public float verticalVelocity;

    float cooldownTimer = 0f;
    bool hasAggro = false;
    bool isBackingOff = false;

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<EnemyMovementManager>();
        anim     = GetComponent<EnemyAnimatorManager>();
        stats    = GetComponent<EnemyStatsManager>();
        agent    = GetComponent<NavMeshAgent>();

        agent.updatePosition = false;
        agent.updateRotation = false;

        if (characterController)
        {
            agent.radius     = characterController.radius;
            agent.height     = characterController.height;
            agent.baseOffset = characterController.center.y;
        }
    }

    protected override void Start()
    {
        base.Start();
        if (!target) target = FindObjectOfType<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();
        if (stats.currentHealth <= 0) return;

        cooldownTimer = Mathf.Max(0f, cooldownTimer - Time.deltaTime);

        float dist = target ? Vector3.Distance(transform.position, target.transform.position) : Mathf.Infinity;

        // Aggro logic
        if (!hasAggro)
        {
            hasAggro = dist <= aggroRadius;
            if (!hasAggro) { movement.Idle(); return; }
        }
        else if (dist >= loseAggroRadius)
        {
            hasAggro = false; movement.Idle(); return;
        }

        // If mid-action (attack/stagger/death), movement manager won't override it
        if (isPerformingAction) return;

        // Decide: Attack or Chase
        if (dist <= attackRange && cooldownTimer <= 0f)
        {
            StartCoroutine(AttackRoutine());
        }
        else
        {
            movement.ChaseTarget(target.transform);
        }

        agent.nextPosition = transform.position;
    }

    IEnumerator AttackRoutine()
    {
        isPerformingAction = true; canMove = false; canRotate = true;

        movement.RotateTowards(target.transform.position);

        // Attack (use root motion if clip has it)
        anim.PlayTargetActionAnimation(attackAnim, true, applyRootMotion: true, canRotate: false, canMove: false);

        // Let most of the animation play (or use an animation event)
        yield return new WaitForSeconds(0.8f);

        // Back off (code-driven step back)
        isBackingOff = true;
        float t = 0f;
        float backSpeed = backOffDistance / Mathf.Max(0.01f, backOffDuration);
        while (t < backOffDuration)
        {
            t += Time.deltaTime;
            movement.BackOffStep(backSpeed);
            yield return null;
        }
        isBackingOff = false;

        cooldownTimer = attackCooldown;
        isPerformingAction = false; canMove = true; canRotate = true;
    }

    // Gravity (same pattern as Player)
    public Vector3 GetGravityDelta()
    {
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = groundedStick;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        return Vector3.up * verticalVelocity * Time.deltaTime;
    }

    public void ProcessDeath()
    {
        if (isPerformingAction) StopAllCoroutines();
        isPerformingAction = true; canMove = false; canRotate = false;

        anim.PlayTargetActionAnimation("Enemy_Death_01", true, applyRootMotion: true, canRotate: false, canMove: false);

        // Disable AI collisions a bit later, or immediately if desired
        StartCoroutine(DisableAfter(5f));
    }

    IEnumerator DisableAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
