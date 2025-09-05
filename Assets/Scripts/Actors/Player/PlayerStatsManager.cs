using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private float staminaRegenTimer = 0;
    private float staminaRegenTimerThreshold = 0.5f;
    private int staminaRegenAmount = 4;

    private void Awake()
    {
        if (playerManager == null)
            playerManager = GetComponent<PlayerManager>();
    }

    public int CalculateStamina(int endurance)
    {
        float stamina = 0;

        stamina = endurance * 10;
        return Mathf.RoundToInt(stamina);
    }

    public int CalculateHealth(int vitality)
    {
        float health = 0;

        health = vitality * 10;
        return Mathf.RoundToInt(health);
    }

    public void RegenerateStamina()
    {
        if (playerManager.isPerformingAction)
            return;

        staminaRegenTimer += Time.deltaTime;

        if (playerManager.currentStamina < playerManager.maxStamina)
        {
            if (staminaRegenTimer >= staminaRegenTimerThreshold)
            {
                playerManager.ChangeStaminaValue(staminaRegenAmount);
                staminaRegenTimer = 0;
            }
        }

    }

    public void CheckHP()
    {
        if (playerManager.currentHealth <= 0 && !playerManager.isDead)
        {
            StartCoroutine(playerManager.ProcessDeath());
        }
    }
}
