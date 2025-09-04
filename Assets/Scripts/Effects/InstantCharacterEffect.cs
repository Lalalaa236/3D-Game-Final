using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantCharacterEffect : ScriptableObject
{
    [Header("Effect ID")]
    public int instantEffectID;

    public virtual void ProcessEffect(PlayerManager playerManager)
    {
        // Debug.Log("Effect processed from base class.");
    }
}
