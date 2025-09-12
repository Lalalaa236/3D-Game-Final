using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Take Damage Effect")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Take Damage Effect Settings")]
    public float physicalDamage = 0;

    [Header("Final Damage Dealt")]
    public int finalDamageDealt = 0;

    [Header("The character causing the damage, if any")]
    public ActorManager characterCausingDamage;

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Direction Damage Taken From")]
    public float angleHitFrom;
    public Vector3 contactPoint;

    public override void ProcessEffect(ActorManager character)
    {
        base.ProcessEffect(character);

        if (character.isDead)
            return;

        CalculateDamage(character);
    }

    private void CalculateDamage(ActorManager character)
    {
        if (characterCausingDamage != null)
        {

        }

        finalDamageDealt = Mathf.RoundToInt(physicalDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }
        
        character.actorStatsManager.ChangeHealthValue(-finalDamageDealt);
    }
}
