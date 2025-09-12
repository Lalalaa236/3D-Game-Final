using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/Idle State")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager character)
    {

        if (character.actorCombatManager.currentTarget != null)
        {
            Debug.Log("Target Found");
        }
        else
        {
            Debug.Log("No Target");
        }
        return this;
    }

}
