using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI States/Idle State")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager character)
    {
        return this;
    }

}
