using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.XR;

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

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float groundedGravity = -2f;
    private float verticalVelocity = 0f;

    [Header("Dodge")]
    [SerializeField] private Vector3 rollDirection;
    [SerializeField] private float rollDistance = 4f;
    [SerializeField] private float rollDuration = 0.45f;
    [SerializeField] private float backstepDistance = 2.0f;
    [SerializeField] private float backstepDuration = 0.30f;

    private bool isRolling = false;
    private Vector3 activeRollDir = Vector3.zero;
    private float rollTimeLeft = 0f;
    private float rollHorizontalSpeed = 0f;

    [Header("NavMesh Settings")]
    [SerializeField] private float maxNavClampDistance = 0.5f;
    [SerializeField] private float raycastPadding = 0.05f;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void GetVerticalAndHorizontalInput()
    {
        verticalMovement = InputManager.instance.verticalMovement;
        horizontalMovement = InputManager.instance.horizontalMovement;
        moveAmount = InputManager.instance.moveAmount;
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
        if (!isRolling && playerManager.canMove == false)
            return;
        GetVerticalAndHorizontalInput();

        moveDir = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDir += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDir.Normalize();
        moveDir.y = 0;

        // Apply gravity
        if (playerManager.characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedGravity;
        }
        verticalVelocity += gravity * gravityMultiplier * Time.deltaTime;

        Vector3 desiredHorizontalDelta = moveDir * sprintingSpeed * Time.deltaTime;
        Vector3 allowedHorizontalDelta = ComputeNavSafeDelta(desiredHorizontalDelta);

        Vector3 finalMove = allowedHorizontalDelta;
        finalMove.y = verticalVelocity * Time.deltaTime;

        playerManager.characterController.Move(finalMove);

        PostSnapToNavMesh();

        // Vector3 movement;
        // if (InputManager.instance.moveAmount > 0.5f)
        // {
        //     movement = moveDir * sprintingSpeed;
        // }
        // else
        // {
        //     movement = moveDir * walkingSpeed;
        // }

        // movement.y = verticalVelocity;
        // playerManager.characterController.Move(movement * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (playerManager.canRotate == false)
            return;

        targetDir = Vector3.zero;
        targetDir = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetDir += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
        {
            targetDir = transform.forward;
        }
        Quaternion newRotation = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = targetRotation;
    }

    public void TryPerformDodge()
    {
        if (playerManager.isPerformingAction || isRolling)
            return;
        if (moveAmount > 0)
        {
            // perform roll
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;

            if (rollDirection.sqrMagnitude < 0.0001f)
                rollDirection = transform.forward;

            rollDirection.y = 0;
            rollDirection.Normalize();
            BeginRoll(rollDirection, rollDistance, rollDuration);

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            playerManager.transform.rotation = playerRotation;

            playerManager.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true, false, false);
        }
        else
        {
            // perform backstep
            playerManager.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true, false, false);
        }

        if (navMeshAgent) navMeshAgent.nextPosition = transform.position;
    }

    private void BeginRoll(Vector3 dir, float distance, float duration)
    {
        isRolling = true;
        playerManager.isPerformingAction = true;

        activeRollDir = dir.normalized;
        rollTimeLeft = Mathf.Max(0.01f, duration);
        rollHorizontalSpeed = distance / rollTimeLeft;

        if (playerManager.characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = groundedGravity;
    }

    private void HandleRollingStep()
    {
        float dt = Time.deltaTime;

        Vector3 desiredHorizontalDelta = activeRollDir * rollHorizontalSpeed * dt;

        if (playerManager.characterController.isGrounded && verticalVelocity < 0f)
            verticalVelocity = groundedGravity;
        verticalVelocity += gravity * gravityMultiplier * dt;

        Vector3 allowedHorizontalDelta = ComputeNavSafeDelta(desiredHorizontalDelta);

        Vector3 finalMove = allowedHorizontalDelta;
        finalMove.y = verticalVelocity * dt;

        playerManager.characterController.Move(finalMove);

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
            transform.position = postHit.position;
            navMeshAgent.nextPosition = postHit.position;
        }
    }
}
