using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [SerializeField] protected Collider damageCollider;
    public float damage = 25f;

    protected Vector3 hitPoint;

    protected List<ActorManager> damagedTargets = new List<ActorManager>();

    private void Awake()
    {
        if (damageCollider == null)
            damageCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        ActorManager target = other.GetComponentInParent<ActorManager>();
        if (target != null)
        {
            hitPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            DamageTarget(target);
        }
    }

    protected virtual void DamageTarget(ActorManager target)
    {
        if (damagedTargets.Contains(target))
            return;

        damagedTargets.Add(target);

        TakeDamageEffect takeDamageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        takeDamageEffect.physicalDamage = damage;

        target.actorStatsManager.ChangeHealthValue(-(int)damage);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        damagedTargets.Clear();
    }
}