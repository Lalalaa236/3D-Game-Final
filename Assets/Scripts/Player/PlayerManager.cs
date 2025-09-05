using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent navMeshAgent;

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
        navMeshAgent = GetComponent<NavMeshAgent>();
        // TurnOffRootMotion();

        navMeshAgent.updatePosition = false; // we'll set nextPosition manually
        navMeshAgent.updateRotation = false;

        // Make agent dimensions match controller (prevents doorway/step weirdness)
        navMeshAgent.radius = characterController.radius;
        navMeshAgent.height = characterController.height;
        navMeshAgent.baseOffset = characterController.center.y;

        if (NavMesh.SamplePosition(transform.position, out var startHit, 2f, NavMesh.AllAreas))
        {
            navMeshAgent.Warp(startHit.position);     // agent internal position
            transform.position = startHit.position; // player transform
        }
        else
        {
            Debug.LogWarning("PlayerManager: Player is not over a NavMesh. Bake/position the player on a walkable area.");
        }

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

        if (navMeshAgent != null) navMeshAgent.nextPosition = transform.position;
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
