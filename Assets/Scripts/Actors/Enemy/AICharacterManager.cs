using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterManager : ActorManager
{
    [Header("Current State")]
    [SerializeField] private AIState currentState;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this);

        if (nextState != null)
        {
            currentState = nextState;
        }
    }
}
