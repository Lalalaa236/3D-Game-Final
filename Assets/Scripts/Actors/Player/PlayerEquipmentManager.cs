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
        EquipWeapon();
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

    public void EquipWeapon()
    {
        if (playerManager.rightHandWeapon != null)
        {
            currentRightHandWeapon = Instantiate(playerManager.rightHandWeapon.weaponPrefab);
            rightHand.EquipWeapon(currentRightHandWeapon);
        }
        if (playerManager.leftHandWeapon != null)
        {
            currentLeftHandWeapon = Instantiate(playerManager.leftHandWeapon.weaponPrefab);
            leftHand.EquipWeapon(currentLeftHandWeapon);
        }
    }
}