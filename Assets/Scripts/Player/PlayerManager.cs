using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    public bool isPerformingAction;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        // TurnOffRootMotion();
        SetCamera();
        SetPlayerManagerInInput();
    }
    
    // private void TurnOffRootMotion()
    // {
    //     animator.applyRootMotion = false;         // parent controls movement
    //     animator.updateMode = AnimatorUpdateMode.Normal;
    //     // keep the model locked under the parent
    //     var model = animator.transform;
    //     model.localPosition = Vector3.zero;
    //     model.localRotation = Quaternion.identity;
    //     model.localScale    = Vector3.one;
    // }

    // private void Start()
    // {

    // }

    private void Update()
    {
        playerMovementManager.HandleMovement();
    }

    private void SetCamera()
    {
        PlayerCamera.instance.playerManager = this;
    }

    private void SetPlayerManagerInInput()
    {
        Debug.Log("Setting Player Manager in Input Manager");
        InputManager.instance.playerManager = this;
    }

    private void LateUpdate()
    {
        PlayerCamera.instance.HandleCameraAction();
    }
}
