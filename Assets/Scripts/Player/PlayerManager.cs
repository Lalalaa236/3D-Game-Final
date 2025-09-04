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
        SetCamera(); 
    }

    private void Update()
    {
        playerMovementManager.HandleMovement();
    }

    private void SetCamera()
    {
        PlayerCamera.instance.playerManager = this;
    }

    private void LateUpdate()
    {
        PlayerCamera.instance.HandleCameraAction();
    }
}
