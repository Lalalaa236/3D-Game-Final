using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerStatsManager    playerStatsManager;
    [HideInInspector] public CharacterController   characterController;
    [HideInInspector] public Animator              animator;
    [HideInInspector] public NavMeshAgent          navMeshAgent;

    // Gameplay flags
    public bool isPerformingAction;
    public bool canRotate = true;
    public bool canMove   = true;
    public bool applyRootMotion = false;

    // ── Gravity (centralized, used by movement if you call GetGravityDelta) ─────────
    [Header("Gravity")]
    public float gravity = -20f;        // try -9.81..-30
    public float groundedStick = -2f;   // small downward force to keep grounded
    [HideInInspector] public float verticalVelocity;

    // ── Stamina / Health (from StatsManager + UI) ───────────────────────────────────
    [Header("Stamina")]
    public int currentStamina;
    public int maxStamina;
    [SerializeField] private int   endurance = 10;
    [SerializeField] private float staminaRegenTimer = 0f;
    [SerializeField] private float staminaRegenTimerThreshold = 0.5f;
    [SerializeField] private int   staminaRegenAmount = 4;

    [Header("Health")]
    public int currentHealth;
    public int maxHealth;
    [SerializeField] private int vitality = 10;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Cache components
        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerStatsManager    = GetComponent<PlayerStatsManager>();
        characterController   = GetComponent<CharacterController>();
        animator              = GetComponent<Animator>();
        navMeshAgent          = GetComponent<NavMeshAgent>();

        // Hook camera & input
        PlayerCamera.instance.playerManager = this;
        InputManager.instance.playerManager = this;

        // NavMeshAgent is for nav-legal clamping; CharacterController drives motion
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;

        // Match agent to controller (prevents doorway weirdness)
        if (characterController != null)
        {
            navMeshAgent.radius     = characterController.radius;
            navMeshAgent.height     = characterController.height;
            navMeshAgent.baseOffset = characterController.center.y;
        }

        // Ensure we start on the NavMesh if possible
        if (NavMesh.SamplePosition(transform.position, out var startHit, 2f, NavMesh.AllAreas))
        {
            navMeshAgent.Warp(startHit.position);  // sync agent
            transform.position = startHit.position; // sync transform
        }
        else
        {
            Debug.LogWarning("PlayerManager: Player is not over a NavMesh. Bake/position the player on a walkable area.");
        }

        // Stats + UI init (if present)
        if (playerStatsManager != null)
        {
            maxStamina     = playerStatsManager.CalculateStamina(endurance);
            currentStamina = maxStamina;
            if (PlayerUIManager.instance != null)
                PlayerUIManager.instance.playerHUDManager.SetMaxStaminaBarValue(maxStamina);

            maxHealth     = playerStatsManager.CalculateHealth(vitality);
            currentHealth = maxHealth;
            if (PlayerUIManager.instance != null)
                PlayerUIManager.instance.playerHUDManager.SetMaxHealthBarValue(maxHealth);
        }
    }

    private void Update()
    {
        playerMovementManager?.HandleMovement();
        RegenerateStamina();

        // Keep NavMeshAgent in sync with the capsule’s transform
        if (navMeshAgent != null) navMeshAgent.nextPosition = transform.position;
    }

    private void LateUpdate()
    {
        PlayerCamera.instance.HandleCameraAction();
    }

    // ── Public helpers ──────────────────────────────────────────────────────────────
    public void ChangeStaminaValue(int value)
    {
        currentStamina = Mathf.Clamp(currentStamina + value, 0, maxStamina);
        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.playerHUDManager.SetNewStaminaBarValue(0, currentStamina);
    }

    public void ChangeHealthValue(int value)
    {
        currentHealth = Mathf.Clamp(currentHealth + value, 0, maxHealth);
        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.playerHUDManager.SetNewHealthBarValue(0, currentHealth);
    }

    public void RegenerateStamina()
    {
        if (isPerformingAction) { staminaRegenTimer = 0f; return; }

        staminaRegenTimer += Time.deltaTime;
        if (currentStamina < maxStamina && staminaRegenTimer >= staminaRegenTimerThreshold)
        {
            ChangeStaminaValue(staminaRegenAmount);
            staminaRegenTimer = 0f;
        }
    }

    /// <summary>
    /// Centralized gravity delta for CharacterController.Move().
    /// Call this from movement instead of duplicating gravity logic.
    /// </summary>
    public Vector3 GetGravityDelta()
    {
        if (characterController != null && characterController.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = groundedStick;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // v = v + g*dt
        }
        return Vector3.up * verticalVelocity * Time.deltaTime;
    }
}
