using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Take Damage Effect Settings")]
    public float physicalDamage = 0;

    [Header("The character causing the damage, if any")]
    public ActorManager characterCausingDamage;

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    public override void ProcessEffect(ActorManager character)
    {
        base.ProcessEffect(character);
    }
}
