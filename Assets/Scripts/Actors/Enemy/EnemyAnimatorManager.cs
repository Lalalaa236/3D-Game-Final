using UnityEngine;

public class EnemyAnimatorManager : ActorAnimatorManager
{
    private EnemyManager enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<EnemyManager>();
    }

    // Use root motion only when performing special actions (e.g., attack)
    void OnAnimatorMove()
    {
        if (!enemy.isPerformingAction) return;
        if (!enemy.animator.applyRootMotion) return;

        Vector3 delta = enemy.animator.deltaPosition;
        delta.y = 0f;
        enemy.characterController.Move(delta + enemy.GetGravityDelta());
        enemy.transform.rotation *= enemy.animator.deltaRotation;
    }
}
