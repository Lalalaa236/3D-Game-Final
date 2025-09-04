using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
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
        if (next.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
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
        HandleMovementInput();
        HandleCameraInput();
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
    }

    private void HandleCameraInput()
    {
        verticalCamera = cameraVector.y;
        horizontalCamera = cameraVector.x;
    }
}
