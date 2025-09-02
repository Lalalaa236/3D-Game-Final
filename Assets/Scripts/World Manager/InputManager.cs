using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    GameInput inputActions;

    [SerializeField] Vector2 movementVector;
    [SerializeField] float moveAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void Start()
    {
        instance.enabled = false;
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        if (next.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        else
        {
            instance.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new GameInput();
            inputActions.Movement.Move.performed += i => movementVector = i.ReadValue<Vector2>();
        }
        inputActions.Enable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void Update()
    {
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        // Clamp movement
        moveAmount = Mathf.Clamp01(Mathf.Abs(movementVector.x) + Mathf.Abs(movementVector.y));

        if (moveAmount <= 0.5f && moveAmount > 0f)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1f)
        {
            moveAmount = 1f;
        }
    }
}
