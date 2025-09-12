using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : ActorEffectsManager
{
    [SerializeField] InstantCharacterEffect currentEffect;
    [SerializeField] bool processEffect = false;

    private void Update()
    {
        if (processEffect)
        {
            processEffect = false;
            InstantCharacterEffect effectToProcess = Instantiate(currentEffect);
            ProcessInstantEffect(effectToProcess);
        }
    }
}
