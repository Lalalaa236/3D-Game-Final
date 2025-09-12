using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorCombatManager : MonoBehaviour
{
    ActorManager character;

    [Header("Current Target")]
    public ActorManager currentTarget;

    protected virtual void Awake()
    {
        character = GetComponent<ActorManager>();
    }

    public virtual void SetTarget(ActorManager target)
    {
        currentTarget = target;
    }
}