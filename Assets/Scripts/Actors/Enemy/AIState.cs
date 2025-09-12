using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : ScriptableObject
{
    public virtual AIState Tick(AICharacterManager character)
    {
        //DO SOME LOGIC TO FIND THE PLAYER
        //IF WE HAVE FOUND THE PLAYER, RETURN THE PURSUE STATE INSTEAD
        //IF NOT, CONTINUE TO RETURN IDLE STATE
        return this;
    }
}
