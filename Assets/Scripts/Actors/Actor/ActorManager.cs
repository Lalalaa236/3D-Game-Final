using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public bool isDead = false;

    public CharacterController characterController;
    public ActorStatsManager actorStatsManager;
    [HideInInspector] public Animator animator;
    [HideInInspector] public ActorEffectsManager actorEffectsManager;
    [HideInInspector] public ActorAnimatorManager actorAnimatorManager;

    public bool isPerformingAction;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;

    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
        actorStatsManager = GetComponent<ActorStatsManager>();
        animator = GetComponent<Animator>();
        actorEffectsManager = GetComponent<ActorEffectsManager>();
        actorAnimatorManager = GetComponent<ActorAnimatorManager>();
    }

    protected virtual void Start()
    {
        IgnoreSelfCollision();
    }

    protected virtual void Update()
    {

    }

    protected virtual void LateUpdate()
    {

    }

    protected virtual void FixedUpdate()
    {

    }
    
    protected virtual void IgnoreSelfCollision()
    {
        Collider characterCollider = GetComponent<Collider>();
        Collider[] damageables = GetComponentsInChildren<Collider>();

        List<Collider> ignoredColliders = new List<Collider>();

        foreach (Collider damageable in damageables)
        {
            ignoredColliders.Add(damageable);
        }
        ignoredColliders.Add(characterCollider);

        foreach (Collider col1 in ignoredColliders)
        {
            foreach (Collider col2 in ignoredColliders)
            {
                if (col1 != col2)
                {
                    Physics.IgnoreCollision(col1, col2);
                }
            }
        }
    }
}