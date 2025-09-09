using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : ActorManager
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    // [HideInInspector] public CharacterController characterController;
    [HideInInspector] public PlayerStatsManager playerStatsManager;

    [Header("Gravity")]
    public float gravity = -20f;        // try -9.81 to -30
    public float groundedStick = -2f;   // small downward "stick to ground"
    [HideInInspector] public float verticalVelocity;

    [Header("Stamina")]
    public int currentStamina;
    public int maxStamina;
    private int endurance = 10;

    [Header("Health")]
    public int currentHealth;
    public int maxHealth;
    private int vitality = 10;
    public bool isDead = false;
    protected override void Awake()
    {
        base.Awake();

        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        // characterController = GetComponent<CharacterController>();
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
        playerStatsManager.CheckHP();
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

    protected override void Update()
    {
        base.Update();
        playerMovementManager.HandleMovement();
        playerStatsManager.RegenerateStamina();
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeHealthValue(-10); // test damage
        }
    }

    protected override void LateUpdate()
    {
        PlayerCamera.instance.HandleCameraAction();
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

    public IEnumerator ProcessDeath(bool manuallySelectDeathAnimation = false)
    {
        PlayerUIManager.instance.playerHUDManager.ShowDeathScreen();

        currentHealth = 0;
        isDead = true;

        // RESET FLAGS
        playerAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
        yield return new WaitForSeconds(5f);
        
        // Auto-revive after the delay
        Revive();
    }
    
    public void Revive()
    {
        isDead = false;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        PlayerUIManager.instance.playerHUDManager.SetNewStaminaBarValue(0, currentStamina);
        PlayerUIManager.instance.playerHUDManager.SetNewHealthBarValue(0, currentHealth);
        PlayerUIManager.instance.playerHUDManager.deathScreen.SetActive(false);
        playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
        
        // Destroy the current player before loading new scene to prevent duplicates
        Destroy(gameObject);
        
        // Load new game scene
        StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
    }
}
