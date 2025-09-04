using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
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
}
