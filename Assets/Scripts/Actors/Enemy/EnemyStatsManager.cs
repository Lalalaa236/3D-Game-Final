using UnityEngine;

public class EnemyStatsManager : ActorStatsManager
{
    private EnemyManager enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyManager>();
    }

    public override void ChangeHealthValue(int value)
    {
        base.ChangeHealthValue(value);
        if (currentHealth <= 0)
        {
            enemy.ProcessDeath();
        }
        else if (value < 0)
        {
            enemy.animator.SetTrigger("Stagger");
            enemy.isPerformingAction = true;
            enemy.StartCoroutine(ClearStagger(0.6f));
        }
    }

    private System.Collections.IEnumerator ClearStagger(float t)
    {
        yield return new WaitForSeconds(t);
        if (currentHealth > 0)
            enemy.isPerformingAction = false;
    }
}
