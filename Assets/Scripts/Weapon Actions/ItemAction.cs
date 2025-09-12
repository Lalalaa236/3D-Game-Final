using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Actions/Item Action/Test Action")]
public class ItemAction : ScriptableObject
{
    public int actionID;

    public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, Weapon itemAction)
    {
        Debug.Log("Attempting to perform action: " + actionID + " with item: " + itemAction.itemName);
    }
}
