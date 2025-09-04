using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }
    // PROCESS INSTANT EFFECT
    public void ProcessInstantEffect(InstantCharacterEffect instantEffect)
    {
        instantEffect.ProcessEffect(playerManager);
    }

    private void Update()
    {
        
    }
}
