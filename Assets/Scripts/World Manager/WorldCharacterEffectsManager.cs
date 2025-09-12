using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager instance;
    [SerializeField] private List<InstantCharacterEffect> instantCharacterEffects;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateEffectIDs()
    { 
        for (int i = 0; i < instantCharacterEffects.Count; i++)
        {
            instantCharacterEffects[i].instantEffectID = i;
        }
    }
}
