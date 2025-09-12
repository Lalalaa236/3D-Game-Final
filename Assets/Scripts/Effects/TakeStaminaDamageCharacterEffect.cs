using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Take Stamina Effect")]
public class TakeStaminaDamageCharacterEffect : InstantCharacterEffect
{
    public float staminaDamage;

    public override void ProcessEffect(ActorManager character)
    {
        CalculateStaminaDamage(character);
    }

    public void CalculateStaminaDamage(ActorManager character)
    {
        character.actorStatsManager.ChangeStaminaValue(-(int)staminaDamage);
    }
}
