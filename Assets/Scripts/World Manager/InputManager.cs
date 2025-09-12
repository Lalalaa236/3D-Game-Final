using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public PlayerManager playerManager;
    GameInput inputActions;

    [Header("Movement")]
    [SerializeField] private Vector2 movementVector;
    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    [Header("Camera")]
    [SerializeField] private Vector2 cameraVector;
    public float verticalCamera;
    public float horizontalCamera;

    [Header("Action")]
    [SerializeField] private bool dodgeInput = false;
    [SerializeField] private bool attackInput = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChanged;
        instance.enabled = false;
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        Debug.Log(next.buildIndex);
        if (next.buildIndex == WorldSaveGameManager.instance.GetGameSceneIndex())
        {
            instance.enabled = true;
            Debug.Log("Input Enabled");
        }
        else
        {
            instance.enabled = false;
            Debug.Log("Input Disabled");
        }
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new GameInput();
            inputActions.Movement.Move.performed += i => movementVector = i.ReadValue<Vector2>();
            inputActions.Camera.Move.performed += i => cameraVector = i.ReadValue<Vector2>();
            inputActions.Actions.Dodge.performed += i => dodgeInput = true;
            inputActions.Actions.Attack.performed += i => attackInput = true;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void Update()
    {
        TickAllInputs();
    }

    private void TickAllInputs()
    {
        HandleMovementInput();
        HandleCameraInput();
        HandleDodgeInput();
        HandleAttackInput();
    }

    private void HandleMovementInput()
    {
        verticalMovement = movementVector.y;
        horizontalMovement = movementVector.x;

        // Clamp movement
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalMovement) + Mathf.Abs(verticalMovement));

        if (moveAmount <= 0.5f && moveAmount > 0f)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1f)
        {
            moveAmount = 1f;
        }

        // Update animator values
        if (playerManager != null)
        {
            // Not locked on target
            playerManager.playerAnimatorManager.UpdateAnimatorValues(0, moveAmount);
        }
    }

    private void HandleCameraInput()
    {
        verticalCamera = cameraVector.y;
        horizontalCamera = cameraVector.x;
    }

    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;

            // Handle dodge action
            playerManager.playerMovementManager.TryPerformDodge();
        }
    }

    private void HandleAttackInput()
    {
        if (attackInput)
        {
            attackInput = false;

            // Handle attack action
            playerManager.playerCombatManager.PerformWeaponBasedAction(playerManager.rightHandWeapon.attackAction,
                                                                        playerManager.playerCombatManager.currentWeapon);
        }
    }
}
