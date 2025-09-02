using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerMovementManager : MonoBehaviour
{
    private PlayerManager playerManager;

    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    private Vector3 moveDir;
    private Vector3 targetDir;

    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintingSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private void Awake()
    {
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
        GetVerticalAndHorizontalInput();

        moveDir = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDir += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDir.Normalize();
        moveDir.y = 0;

        if (InputManager.instance.moveAmount > 0.5)
        {
            playerManager.characterController.Move(moveDir * sprintingSpeed * Time.deltaTime);
        }
        else if (InputManager.instance.moveAmount <= 0.5f)
        {
            playerManager.characterController.Move(moveDir * walkingSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        targetDir = Vector3.zero;
        targetDir = PlayerCamera.instance.playerCamera.transform.forward * verticalMovement;
        targetDir += PlayerCamera.instance.playerCamera.transform.right * horizontalMovement;
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
}
