using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerMovementManager playerMovementManager;
    public CharacterController characterController;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        playerMovementManager = GetComponent<PlayerMovementManager>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        playerMovementManager.HandleMovement();
    }
}
