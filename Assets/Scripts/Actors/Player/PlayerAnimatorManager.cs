using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : ActorAnimatorManager
{
    private PlayerManager playerManager;

    protected override void Awake()
    {
        base.Awake();
        playerManager = GetComponent<PlayerManager>();
    }
    // public void PlayTargetActionAnimation(string targetAnim, bool isPerformingAction, bool applyRootMotion = false, bool canRotate = false, bool canMove = false)
    // {
    //     // playerManager.animator.SetBool("isPerformingAction", isPerformingAction);
    //     playerManager.applyRootMotion = applyRootMotion;
    //     playerManager.animator.CrossFade(targetAnim, 0.2f);
    //     playerManager.isPerformingAction = isPerformingAction;
    //     playerManager.canRotate = canRotate;
    //     playerManager.canMove = canMove;
    // }

    private void OnAnimatorMove()
    {
        // if (playerManager.applyRootMotion)
        // {
        //     Vector3 deltaPosition = playerManager.animator.deltaPosition;
        //     playerManager.characterController.Move(deltaPosition);
        //     playerManager.transform.rotation *= playerManager.animator.deltaRotation;
        // }
        // Only during special actions (e.g., roll) that enable root motion
        if (!playerManager.isPerformingAction) return;
        if (!playerManager.animator.applyRootMotion) return; // this is what you set when starting roll

        // Animation-driven horizontal
        Vector3 delta = playerManager.animator.deltaPosition;
        delta.y = 0f;

        // Same gravity used by locomotion
        Vector3 vertical = playerManager.GetGravityDelta();

        playerManager.characterController.Move(delta + vertical);
        playerManager.transform.rotation *= playerManager.animator.deltaRotation;
    }
}