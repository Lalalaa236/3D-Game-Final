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

    private Vector3 moveDirection;

    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float sprintingSpeed = 5f;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void HandleMovement()
    {
        // Handle player movement input and apply it to the player character
        HandleGroundedMovement();
    }

    private void HandleGroundedMovement()
    {
        GetVerticalAndHorizontalInput();
        
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (InputManager.instance.moveAmount > 0.5)
        {
            playerManager.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else if (InputManager.instance.moveAmount <= 0.5f)
        {
            playerManager.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    private void GetVerticalAndHorizontalInput()
    {
        verticalMovement = InputManager.instance.verticalMovement;
        horizontalMovement = InputManager.instance.horizontalMovement;
        moveAmount = InputManager.instance.moveAmount;
    }
}
