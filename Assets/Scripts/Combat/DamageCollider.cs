using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public float damage = 25f;

    protected Vector3 hitPoint;

    protected List<ActorManager> damagedTargets = new List<ActorManager>();

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

        target.actorStatsManager.ChangeHealthValue(-(int)damage);
    }
}