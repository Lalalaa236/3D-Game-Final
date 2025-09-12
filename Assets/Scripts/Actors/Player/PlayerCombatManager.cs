using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : ActorCombatManager
{
    PlayerManager player;
    
    public Item currentWeapon;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public void PerformWeaponBasedAction(ItemAction weaponAction, Weapon weaponPerformingAction)
    {
        weaponAction.AttemptToPerformAction(player, weaponPerformingAction);
    }
}

