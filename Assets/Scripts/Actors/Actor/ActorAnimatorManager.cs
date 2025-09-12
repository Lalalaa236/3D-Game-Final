using UnityEngine;
public class ActorAnimatorManager : MonoBehaviour
{
    private ActorManager actorManager;

    protected virtual void Awake()
    {
        actorManager = GetComponent<ActorManager>();
    }

    public void UpdateAnimatorValues(float horizontalValue, float verticalValue)
    {
        actorManager.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        actorManager.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
    }

    public void PlayTargetActionAnimation(string targetAnim, bool isPerformingAction, bool applyRootMotion = false, bool canRotate = false, bool canMove = false)
    {
        // actorManager.animator.SetBool("isPerformingAction", isPerformingAction);
        actorManager.animator.applyRootMotion = applyRootMotion;
        actorManager.animator.CrossFade(targetAnim, 0.2f);
        actorManager.isPerformingAction = isPerformingAction;
        actorManager.canRotate = canRotate;
        actorManager.canMove = canMove;
    }

    public void PlayTargetAttackActionAnimation(string targetAnim, bool isPerformingAction, bool applyRootMotion = false, bool canRotate = false, bool canMove = false)
    {
        // actorManager.animator.SetBool("isPerformingAction", isPerformingAction);
        actorManager.animator.applyRootMotion = applyRootMotion;
        actorManager.animator.CrossFade(targetAnim, 0.2f);
        actorManager.isPerformingAction = isPerformingAction;
        actorManager.canRotate = canRotate;
        actorManager.canMove = canMove;
    }
}