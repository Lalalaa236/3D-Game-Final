using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldActionManager : MonoBehaviour
{
    public static WorldActionManager instance;

    [Header("Weapon Action")]
    public ItemAction[] weaponItemAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        for (int i = 0; i < weaponItemAction.Length; i++)
        {
            weaponItemAction[i].actionID = i;
        }
    }

    public ItemAction GetWeaponItemAction(int actionID)
    {
        return weaponItemAction.FirstOrDefault(action => action.actionID == actionID);
    }

}
