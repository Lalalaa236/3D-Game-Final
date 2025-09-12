using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantCharacterEffect : ScriptableObject
{
    [Header("Effect Settings")]
    public int instantEffectID;

    public virtual void ProcessEffect(ActorManager character)
    { 
        
    }
}
