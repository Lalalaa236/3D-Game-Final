using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/Weapon")]
public class Weapon : Item
{
    public GameObject weaponPrefab;
    public int weaponDamage = 0;
    public int staminaCost = 0;
    public ItemAction attackAction;
}