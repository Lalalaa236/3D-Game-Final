using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    private PlayerManager playerManager;

    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    private Vector3 moveDirection;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void HandleMovement()
    {
        // Handle player movement input and apply it to the player character
    }

    private void HandleGroundedMovement()
    {
        
    }
}
