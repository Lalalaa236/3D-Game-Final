using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovementManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private NavMeshAgent navMeshAgent;

    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    private Vector3 moveDir;
    private Vector3 targetDir;

    [Header("Movement Speeds")]
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintingSpeed = 5f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Dodge / Roll")]
    [SerializeField] private Vector3 rollDirection;
    [SerializeField] private float rollDistance = 4f;
    [SerializeField] private float rollDuration = 0.45f;
    [SerializeField] private float backstepDistance = 2.0f;
    [SerializeField] private float backstepDuration = 0.30f;
    [SerializeField] private float dodgeStaminaCost = 15f;

    private bool   isRolling = false;
    private Vector3 activeRollDir = Vector3.zero;
    private float  rollTimeLeft = 0f;
    private float  rollHorizontalSpeed = 0f;

    [Header("NavMesh Settings")]
    [SerializeField] private float maxNavClampDistance = 0.5f;
    [SerializeField] private float raycastPadding = 0.05f;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        navMeshAgent  = GetComponent<NavMeshAgent>();
    }

    private void GetVerticalAndHorizontalInput()
    {
        verticalMovement   = InputManager.instance.verticalMovement;
        horizontalMovement = InputManager.instance.horizontalMovement;
        moveAmount         = InputManager.instance.moveAmount;
    }

    public void HandleMovement()
    {
        if (isRolling)
        {
            HandleRollingStep();
            HandleRollRotation();
        }
        else
        {
            HandleGroundedMovement();
            HandleRotation();
        }

        if (navMeshAgent) navMeshAgent.nextPosition = transform.position;
    }

    private void HandleGroundedMovement()
    {
        if (!isRolling && playerManager.canMove == false) return;

        GetVerticalAndHorizontalInput();

        // Camera-relative XZ direction
        moveDir  = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDir += PlayerCamera.instance.transform.right   * horizontalMovement;
        moveDir.y = 0f;
        moveDir.Normalize();

        // Walk or Sprint
        float speed = (moveAmount > 0.5f) ? sprintingSpeed : walkingSpeed;

        // --- Horizontal (nav-safe) ---
        Vector3 desiredHorizontalDelta = moveDir * speed * Time.deltaTime;
        Vector3 allowedHorizontalDelta = ComputeNavSafeDelta(desiredHorizontalDelta);

        // --- Vertical (centralized gravity from PlayerManager) ---
        Vector3 verticalDelta = playerManager.GetGravityDelta();

        // One move call
        playerManager.characterController.Move(allowedHorizontalDelta + verticalDelta);

        PostSnapToNavMesh();
    }

    private void HandleRotation()
    {
        if (playerManager.canRotate == false) return;

        targetDir  = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetDir += PlayerCamera.instance.cameraObject.transform.right   * horizontalMovement;
        targetDir.y = 0f;

        if (targetDir.sqrMagnitude < 0.0001f)
            targetDir = transform.forward;

        Quaternion newRotation    = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void TryPerformDodge()
    {
        if (playerManager.isPerformingAction || isRolling) return;

        // stamina gate (from version B)
        if (playerManager.currentStamina < dodgeStaminaCost) return;

        if (moveAmount > 0f)
        {
            // roll in input direction (camera-relative)
            rollDirection  = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right   * horizontalMovement;

            if (rollDirection.sqrMagnitude < 0.0001f)
                rollDirection = transform.forward;

            rollDirection.y = 0f;
            rollDirection.Normalize();

            BeginRoll(rollDirection, rollDistance, rollDuration);

            // face roll direction
            transform.rotation = Quaternion.LookRotation(rollDirection);

            // play animation (root motion translation OFF to avoid double-move)
            playerManager.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true, false, false);
        }
        else
        {
            // backstep anim only (you can make it physical by BeginRoll(-transform.forward,...))
            playerManager.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true, false, false);
        }

        // stamina spend
        playerManager.ChangeStaminaValue(-Mathf.RoundToInt(dodgeStaminaCost));

        if (navMeshAgent) navMeshAgent.nextPosition = transform.position;
    }

    private void BeginRoll(Vector3 dir, float distance, float duration)
    {
        isRolling = true;
        playerManager.isPerformingAction = true;

        activeRollDir       = dir.normalized;
        rollTimeLeft        = Mathf.Max(0.01f, duration);
        rollHorizontalSpeed = distance / rollTimeLeft;
    }

    private void HandleRollingStep()
    {
        float dt = Time.deltaTime;

        // Horizontal desire for the roll (nav-safe)
        Vector3 desiredHorizontalDelta = activeRollDir * rollHorizontalSpeed * dt;
        Vector3 allowedHorizontalDelta = ComputeNavSafeDelta(desiredHorizontalDelta);

        // Gravity during roll (centralized)
        Vector3 verticalDelta = playerManager.GetGravityDelta();

        playerManager.characterController.Move(allowedHorizontalDelta + verticalDelta);

        PostSnapToNavMesh();

        rollTimeLeft -= dt;
        if (rollTimeLeft <= 0f)
        {
            isRolling = false;
            playerManager.isPerformingAction = false;
        }
    }

    private void HandleRollRotation()
    {
        if (activeRollDir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(activeRollDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
        }
    }

    // ── Nav helpers ────────────────────────────────────────────────────────────────
    private Vector3 ComputeNavSafeDelta(Vector3 desiredHorizontalDelta)
    {
        Vector3 origin  = transform.position;
        Vector3 desired = origin + desiredHorizontalDelta;

        Vector3 allowed = Vector3.zero;

        bool haveFrom = NavMesh.SamplePosition(origin,  out var fromHit, maxNavClampDistance, NavMesh.AllAreas);
        bool haveTo   = NavMesh.SamplePosition(desired, out var toHit,   maxNavClampDistance, NavMesh.AllAreas);

        if (haveFrom && haveTo)
        {
            if (NavMesh.Raycast(fromHit.position + Vector3.up * raycastPadding,
                                toHit.position   + Vector3.up * raycastPadding,
                                out var rayHit, NavMesh.AllAreas))
            {
                Vector3 attempted = toHit.position - fromHit.position;
                Vector3 slide     = Vector3.ProjectOnPlane(attempted, rayHit.normal);
                allowed = slide;
            }
            else
            {
                allowed = toHit.position - fromHit.position;
            }
        }
        else
        {
            if (NavMesh.SamplePosition(desired, out var clampHit, maxNavClampDistance, NavMesh.AllAreas))
                allowed = clampHit.position - origin;
            else
                allowed = Vector3.zero;
        }

        return allowed;
    }

    private void PostSnapToNavMesh()
    {
        if (navMeshAgent && NavMesh.SamplePosition(transform.position, out var postHit, maxNavClampDistance, NavMesh.AllAreas))
        {
            transform.position       = postHit.position;
            navMeshAgent.nextPosition = postHit.position;
        }
    }
}
