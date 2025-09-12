using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : ActorManager
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;

    [Header("Gravity")]
    public float gravity = -20f;        // try -9.81 to -30
    public float groundedStick = -2f;   // small downward "stick to ground"
    [HideInInspector] public float verticalVelocity;
    [HideInInspector] public bool isDead = false;

    [Header("Weapon")]
    public Weapon rightHandWeapon;
    public Weapon leftHandWeapon;
    public int rightIndex = 0;

    public Weapon[] switchableRightHandWeapons = new Weapon[2];
    protected override void Awake()
    {
        base.Awake();

        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        // characterController = GetComponent<CharacterController>();
        playerStatsManager = actorStatsManager as PlayerStatsManager;
        animator = GetComponent<Animator>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        // TurnOffRootMotion();
        PlayerCamera.instance.playerManager = this;
        InputManager.instance.playerManager = this;


        PlayerUIManager.instance.playerHUDManager.SetMaxStaminaBarValue(playerStatsManager.maxStamina);

        PlayerUIManager.instance.playerHUDManager.SetMaxHealthBarValue(playerStatsManager.maxHealth);
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
            playerStatsManager.ChangeHealthValue(-10); // test damage
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            playerEquipmentManager.SwitchRightWeapon();
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

        playerStatsManager.currentHealth = 0;
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
        playerStatsManager.currentHealth = playerStatsManager.maxHealth;
        playerStatsManager.currentStamina = playerStatsManager.maxStamina;
        PlayerUIManager.instance.playerHUDManager.SetNewStaminaBarValue(0, playerStatsManager.currentStamina);
        PlayerUIManager.instance.playerHUDManager.SetNewHealthBarValue(0, playerStatsManager.currentHealth);
        PlayerUIManager.instance.playerHUDManager.deathScreen.SetActive(false);
        playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
        
        // Destroy the current player before loading new scene to prevent duplicates
        Destroy(gameObject);
        
        // Load new game scene
        StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
    }
}
