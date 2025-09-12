using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorEffectsManager : MonoBehaviour
{
    ActorManager character;

    protected virtual void Awake()
    {
        character = GetComponent<ActorManager>();
    }
    public void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }
}
