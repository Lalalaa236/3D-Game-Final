using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStatsManager : MonoBehaviour
{
    public int currentHealth = 100;
    public int maxHealth = 100;
    public int currentStamina = 100;
    public int maxStamina = 100;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    public virtual void ChangeStaminaValue(int value)
    {
        currentStamina += value;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    public virtual void ChangeHealthValue(int value)
    {
        currentHealth += value;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}