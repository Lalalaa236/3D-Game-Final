using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public Animator animator;

    public bool isPerformingAction;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;

    [Header("Gravity")]
    public float gravity = -20f;        // try -9.81 to -30
    public float groundedStick = -2f;   // small downward "stick to ground"
    [HideInInspector] public float verticalVelocity;

    [Header("Stamina")]
    public int currentStamina;
    public int maxStamina;
    private int endurance = 10;

    [SerializeField] private float staminaRegenTimer = 0;
    private float staminaRegenTimerThreshold = 0.5f;
    private int staminaRegenAmount = 4;

    [Header("Health")]
    public int currentHealth;
    public int maxHealth;
    private int vitality = 10;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        characterController = GetComponent<CharacterController>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        animator = GetComponent<Animator>();
        // TurnOffRootMotion();
        PlayerCamera.instance.playerManager = this;
        InputManager.instance.playerManager = this;

        maxStamina = playerStatsManager.CalculateStamina(endurance);
        currentStamina = maxStamina;
        PlayerUIManager.instance.playerHUDManager.SetMaxStaminaBarValue(maxStamina);

        maxHealth = playerStatsManager.CalculateHealth(vitality);
        currentHealth = maxHealth;
        PlayerUIManager.instance.playerHUDManager.SetMaxHealthBarValue(maxHealth);
    }

    public void ChangeStaminaValue(int value)
    {
        currentStamina += value;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        PlayerUIManager.instance.playerHUDManager.SetNewStaminaBarValue(0, currentStamina);
    }

    public void ChangeHealthValue(int value)
    {
        currentHealth += value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        PlayerUIManager.instance.playerHUDManager.SetNewHealthBarValue(0, currentHealth);
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
        RegenerateStamina();
    }

    private void LateUpdate()
    {
        PlayerCamera.instance.HandleCameraAction();
    }

    public void RegenerateStamina()
    {
        if (isPerformingAction)
            return;

        staminaRegenTimer += Time.deltaTime;

        if (currentStamina < maxStamina)
        {
            if (staminaRegenTimer >= staminaRegenTimerThreshold)
            {
                ChangeStaminaValue(staminaRegenAmount);
                staminaRegenTimer = 0;
            }
        }

    }
    
    public Vector3 GetGravityDelta()
    {
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = groundedStick;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;   // v = v + g*dt
        }
        return Vector3.up * verticalVelocity * Time.deltaTime;
    }
}
