using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : ActorStatsManager
{
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private float staminaRegenTimer = 0;
    private float staminaRegenTimerThreshold = 0.5f;
    private int staminaRegenAmount = 4;

    protected override void Awake()
    {
        base.Awake();

        if (playerManager == null)
            playerManager = GetComponent<PlayerManager>();
    }

    public void RegenerateStamina()
    {
        if (playerManager.isPerformingAction)
            return;

        staminaRegenTimer += Time.deltaTime;

        if (currentStamina < maxStamina)
        {
            Debug.Log("Regenerating Stamina from " + currentStamina);
            if (staminaRegenTimer >= staminaRegenTimerThreshold)
            {
                ChangeStaminaValue(staminaRegenAmount);
                staminaRegenTimer = 0;
            }
        }

    }

    public void CheckHP()
    {
        if (currentHealth <= 0 && !playerManager.isDead)
        {
            StartCoroutine(playerManager.ProcessDeath());
        }
    }

    public override void ChangeHealthValue(int value)
    {
        base.ChangeHealthValue(value);
        PlayerUIManager.instance.playerHUDManager.SetNewHealthBarValue(0, currentHealth);
        CheckHP();
    }

    public override void ChangeStaminaValue(int value)
    {
        base.ChangeStaminaValue(value);
        PlayerUIManager.instance.playerHUDManager.SetNewStaminaBarValue(0, currentStamina);
    }
}
