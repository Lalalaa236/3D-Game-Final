using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerManager : ActorManager
{
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerStatsManager    playerStatsManager;
    [HideInInspector] public NavMeshAgent          navMeshAgent;

    [Header("Gravity")]
    public float gravity = -20f;        // try -9.81..-30
    public float groundedStick = -2f;   // small downward "stick to ground"
    [HideInInspector] public float verticalVelocity;

    [Header("State")]
    public bool isDead = false;

    protected override void Awake()
    {
        base.Awake(); // let ActorManager cache animator/characterController/actorStatsManager, etc.

        // Components specific to Player
        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerStatsManager    = actorStatsManager as PlayerStatsManager;
        navMeshAgent          = GetComponent<NavMeshAgent>();

        // Hook camera & input
        PlayerCamera.instance.playerManager = this;
        InputManager.instance.playerManager = this;

        // Persist unless explicitly destroyed (e.g., on death)
        DontDestroyOnLoad(gameObject);

        // NavMeshAgent is used for nav-legal clamping; CharacterController drives motion
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;

        // Mirror agent dimensions to controller to avoid doorway/step issues
        if (characterController != null)
        {
            navMeshAgent.radius     = characterController.radius;
            navMeshAgent.height     = characterController.height;
            navMeshAgent.baseOffset = characterController.center.y;
        }

        // Ensure we start on the NavMesh if possible
        if (NavMesh.SamplePosition(transform.position, out var startHit, 2f, NavMesh.AllAreas))
        {
            navMeshAgent.Warp(startHit.position);
            transform.position = startHit.position;
        }
        else
        {
            Debug.LogWarning("PlayerManager: Player is not over a NavMesh. Bake/position the player on a walkable area.");
        }

        // Initialize UI from PlayerStatsManager (refactor flow)
        if (playerStatsManager != null && PlayerUIManager.instance != null)
        {
            PlayerUIManager.instance.playerHUDManager.SetMaxStaminaBarValue(playerStatsManager.maxStamina);
            PlayerUIManager.instance.playerHUDManager.SetMaxHealthBarValue(playerStatsManager.maxHealth);
        }
    }

    protected override void Update()
    {
        base.Update();

        // Drive movement
        playerMovementManager?.HandleMovement();

        // Stamina regen handled by the stats manager (refactor flow)
        playerStatsManager?.RegenerateStamina();

        // Keep NavMeshAgent synced to the capsule transform
        if (navMeshAgent != null) navMeshAgent.nextPosition = transform.position;

        // Test damage key (optional)
        if (Input.GetKeyDown(KeyCode.K))
        {
            playerStatsManager?.ChangeHealthValue(-10);
        }
    }

    protected override void LateUpdate()
    {
        PlayerCamera.instance.HandleCameraAction();
    }

    /// <summary>
    /// Centralized gravity delta for CharacterController.Move().
    /// </summary>
    public Vector3 GetGravityDelta()
    {
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f) verticalVelocity = groundedStick;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        return Vector3.up * verticalVelocity * Time.deltaTime;
    }

    // ── Compatibility wrappers (old code may still call these) ─────────────────────
    public void ChangeStaminaValue(int value)
    {
        if (playerStatsManager == null) return;
        playerStatsManager.currentStamina = Mathf.Clamp(playerStatsManager.currentStamina + value, 0, playerStatsManager.maxStamina);
        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.playerHUDManager.SetNewStaminaBarValue(0, playerStatsManager.currentStamina);
    }

    public void ChangeHealthValue(int value)
    {
        if (playerStatsManager == null) return;
        playerStatsManager.currentHealth = Mathf.Clamp(playerStatsManager.currentHealth + value, 0, playerStatsManager.maxHealth);
        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.playerHUDManager.SetNewHealthBarValue(0, playerStatsManager.currentHealth);
    }

    // ── Death / Revive flow (from refactor) ────────────────────────────────────────
    public IEnumerator ProcessDeath(bool manuallySelectDeathAnimation = false)
    {
        PlayerUIManager.instance.playerHUDManager.ShowDeathScreen();

        if (playerStatsManager != null) playerStatsManager.currentHealth = 0;
        isDead = true;

        playerAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
        yield return new WaitForSeconds(5f);

        Revive();
    }

    public void Revive()
    {
        isDead = false;

        if (playerStatsManager != null)
        {
            playerStatsManager.currentHealth  = playerStatsManager.maxHealth;
            playerStatsManager.currentStamina = playerStatsManager.maxStamina;

            if (PlayerUIManager.instance != null)
            {
                PlayerUIManager.instance.playerHUDManager.SetNewStaminaBarValue(0, playerStatsManager.currentStamina);
                PlayerUIManager.instance.playerHUDManager.SetNewHealthBarValue(0,  playerStatsManager.currentHealth);
                PlayerUIManager.instance.playerHUDManager.deathScreen.SetActive(false);
            }
        }

        playerAnimatorManager.PlayTargetActionAnimation("Empty", false);

        // Destroy the current player before loading new scene to prevent duplicates
        Destroy(gameObject);

        // Load new game scene
        StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
    }
}
