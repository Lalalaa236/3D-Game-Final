using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDManager : MonoBehaviour
{
    [SerializeField] private UI_StatBar staminaBar;

    public void SetNewStaminaBarValue(int oldValue, int newValue)
    {
        staminaBar.SetStat(newValue);
    }

    public void SetMaxStaminaBarValue(int maxValue)
    {
        staminaBar.SetMaxStat(maxValue);
    }
}
