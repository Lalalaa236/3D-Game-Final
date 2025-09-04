using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Instant/Take Stamina Damage Effect")]
public class TakeStaminaDamageEffect : InstantCharacterEffect
{
    public int staminaDamageAmount;

    public override void ProcessEffect(PlayerManager playerManager)
    {
        base.ProcessEffect(playerManager);
        playerManager.ChangeStaminaValue(-staminaDamageAmount);
    }

    private void CalculateStaminaDamage()
    {
        // Implement stamina damage calculation logic here
    }
}
