using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Actions/Light Attack Weapon Action")]
public class LightAttackWeaponItemAction : ItemAction
{
    [SerializeField] private string lightAttackAnimationName = "Main_Light_Attack_01";
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, Weapon actionWeapon)
    {
        base.AttemptToPerformAction(playerPerformingAction, actionWeapon);

        if (playerPerformingAction.playerStatsManager.currentStamina <= actionWeapon.staminaCost)
            return;

        playerPerformingAction.playerStatsManager.ChangeStaminaValue(-actionWeapon.staminaCost);

        PerformLightAttackAction(playerPerformingAction, actionWeapon);
    }

    private void PerformLightAttackAction(PlayerManager playerPerformingAction, Weapon actionWeapon)
    {
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(lightAttackAnimationName, true, true, true, true);
    }
}
