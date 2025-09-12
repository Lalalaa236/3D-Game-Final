using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    private PlayerManager playerManager;
    public WeaponInstantiator rightHand;
    public WeaponInstantiator leftHand;

    private GameObject currentRightHandWeapon;
    private GameObject currentLeftHandWeapon;

    private void Awake()
    {
        InitSlots();
        playerManager = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        EquipRightWeapon();
        // EquipLeftWeapon();
    }

    public void InitSlots()
    {
        WeaponInstantiator[] weaponSlots = GetComponentsInChildren<WeaponInstantiator>();
        foreach (WeaponInstantiator weaponSlot in weaponSlots)
        {
            if (weaponSlot.slot == WeaponInstantiator.Slot.RightHand)
            {
                rightHand = weaponSlot;
            }
            else if (weaponSlot.slot == WeaponInstantiator.Slot.LeftHand)
            {
                leftHand = weaponSlot;
            }
        }
    }

    public void EquipRightWeapon()
    {
        if (playerManager.rightHandWeapon != null)
        {
            Debug.Log("Equipping right weapon");
            currentRightHandWeapon = Instantiate(playerManager.rightHandWeapon.weaponPrefab);
            rightHand.EquipWeapon(currentRightHandWeapon);
            DamageCollider damageCollider = currentRightHandWeapon.GetComponentInChildren<DamageCollider>();
            damageCollider.damage = playerManager.rightHandWeapon.weaponDamage;
        }
    }
    // public void EquipLeftWeapon()
    // {
    //     if (playerManager.leftHandWeapon != null)
    //     {
    //         currentLeftHandWeapon = Instantiate(playerManager.leftHandWeapon.weaponPrefab);
    //         leftHand.EquipWeapon(currentLeftHandWeapon);
    //         DamageCollider damageCollider = currentLeftHandWeapon.GetComponentInChildren<DamageCollider>();
    //         damageCollider.damage = playerManager.leftHandWeapon.weaponDamage;
    //     }
    // }

    public void SwitchRightWeapon()
    {
        playerManager.playerAnimatorManager.PlayTargetActionAnimation("Switch_Right_Weapon", false, true, true, true);

        Weapon selected = null;
        ++playerManager.rightIndex;
        playerManager.rightIndex = playerManager.rightIndex % playerManager.switchableRightHandWeapons.Length;
        if (playerManager.rightIndex < 0)
        {
            playerManager.rightIndex = 0;
        }

        selected = playerManager.switchableRightHandWeapons[playerManager.rightIndex];
        playerManager.rightHandWeapon = selected;
        playerManager.playerCombatManager.currentWeapon = selected;
        rightHand.DestroyWeapon();
        EquipRightWeapon();
    }

    public void OpenDamageCollider()
    {
        DamageCollider damageCollider = currentRightHandWeapon.GetComponentInChildren<DamageCollider>();
        damageCollider.EnableDamageCollider();
    }

    public void CloseDamageCollider()
    {
        DamageCollider damageCollider = currentRightHandWeapon.GetComponentInChildren<DamageCollider>();
        damageCollider.DisableDamageCollider();
    }
}