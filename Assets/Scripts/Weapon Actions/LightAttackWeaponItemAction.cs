using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Actions/Light Attack Weapon Action")]
public class LightAttackWeaponItemAction : ItemAction
{
    [SerializeField] private string lightAttackAnimationName = "Main_Light_Attack_01";
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, Weapon itemAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, itemAction);

        if (playerPerformingAction.playerStatsManager.currentStamina <= 0)
            return;

        PerformLightAttackAction(playerPerformingAction, itemAction);
    }

    private void PerformLightAttackAction(PlayerManager playerPerformingAction, Item itemAction)
    {
        playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(lightAttackAnimationName, true);
    }
}
