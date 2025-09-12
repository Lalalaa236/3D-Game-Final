using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : ActorCombatManager
{
    [SerializeField] private PlayerManager player;
    
    public Weapon currentWeapon;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }
    
    private void Start()
    {
        currentWeapon = player.rightHandWeapon;
    }

    public void PerformWeaponBasedAction(ItemAction weaponAction, Weapon weaponPerformingAction)
    {

        weaponAction.AttemptToPerformAction(player, weaponPerformingAction);
    }
}

