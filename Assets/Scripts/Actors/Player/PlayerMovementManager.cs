using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerMovementManager : ActorMovementManager
{
    private PlayerManager playerManager;

    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    private Vector3 moveDir;
    private Vector3 targetDir;

    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintingSpeed = 5f;
    [SerializeField] private float rotationSpeed = 15f;
    // [SerializeField] private float groundedVelocity = -20f;
    // [SerializeField] private float groundCheckSphereRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dodge")]
    [SerializeField] private Vector3 rollDirection;
    [SerializeField] private float dodgeStaminaCost = 15f;

    protected override void Awake()
    {
        base.Awake();
        
        playerManager = GetComponent<PlayerManager>();
    }

    private void GetVerticalAndHorizontalInput()
    {
        verticalMovement = InputManager.instance.verticalMovement;
        horizontalMovement = InputManager.instance.horizontalMovement;
        moveAmount = InputManager.instance.moveAmount;
    }

    public void HandleMovement()
    {
        // Handle player movement input and apply it to the player character

        HandleGroundedMovement();
        HandleRotation();
    }

    private void HandleGroundedMovement()
    {
        // if (playerManager.canMove == false)
        //     return;
        // GetVerticalAndHorizontalInput();

        // moveDir = PlayerCamera.instance.transform.forward * verticalMovement;
        // moveDir += PlayerCamera.instance.transform.right * horizontalMovement;
        // moveDir.Normalize();
        // moveDir.y = 0;

        // if (InputManager.instance.moveAmount > 0.5)
        // {
        //     playerManager.characterController.Move(moveDir * sprintingSpeed * Time.deltaTime);
        // }
        // else if (InputManager.instance.moveAmount <= 0.5f)
        // {
        //     playerManager.characterController.Move(moveDir * walkingSpeed * Time.deltaTime);
        // }

        if (playerManager.canMove == false) return;

        GetVerticalAndHorizontalInput();

        // --- horizontal XZ from camera ---
        Vector3 forward = PlayerCamera.instance.transform.forward;
        Vector3 right   = PlayerCamera.instance.transform.right;
        forward.y = right.y = 0f; forward.Normalize(); right.Normalize();

        Vector3 inputDir = (forward * verticalMovement + right * horizontalMovement).normalized;
        float speed = (InputManager.instance.moveAmount > 0.5f) ? sprintingSpeed : walkingSpeed;
        Vector3 horizontal = speed * Time.deltaTime * inputDir;

        // --- vertical from shared gravity ---
        Vector3 vertical = playerManager.GetGravityDelta();

        // --- ONE move this frame ---
        playerManager.characterController.Move(horizontal + vertical);  
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
        if (playerManager.isPerformingAction)
            return;
        if (playerManager.currentStamina < dodgeStaminaCost)
            return;
        if (moveAmount > 0)
        {
            // perform roll
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;


            rollDirection.y = 0;
            rollDirection.Normalize();
            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            playerManager.transform.rotation = playerRotation;

            playerManager.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true, false, false);
        }
        else
        {
            // perform backstep
            playerManager.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true, false, false);
        }
        playerManager.ChangeStaminaValue(-Mathf.RoundToInt(dodgeStaminaCost));
    }

    // private void HandleGroundCheck()
    // {
    //     playerManager.isGrounded = Physics.CheckSphere(playerManager.transform.position, groundCheckSphereRadius, groundLayer);
    // }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, groundCheckSphereRadius);
    // }
}
