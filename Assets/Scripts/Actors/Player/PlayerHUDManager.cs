using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    [SerializeField] private UI_StatBar staminaBar;
    [SerializeField] private UI_StatBar healthBar;
    [SerializeField] public GameObject deathScreen;
    private void Awake()
    {
        deathScreen.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
    }

    public void SetNewStaminaBarValue(int oldValue, int newValue)
    {
        staminaBar.SetStat(newValue);
    }

    public void SetNewHealthBarValue(int oldValue, int newValue)
    {
        healthBar.SetStat(newValue);
    }

    public void SetMaxStaminaBarValue(int maxValue)
    {
        staminaBar.SetMaxStat(maxValue);
    }
    
    public void SetMaxHealthBarValue(int maxValue)
    {
        healthBar.SetMaxStat(maxValue);
    }
}
