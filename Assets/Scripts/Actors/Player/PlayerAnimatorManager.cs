using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void UpdateAnimatorValues(float horizontalValue, float verticalValue)
    {
        playerManager.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        playerManager.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
    }

    public void PlayTargetActionAnimation(string targetAnim, bool isPerformingAction, bool applyRootMotion = false, bool canRotate = false, bool canMove = false)
    {
        // playerManager.animator.SetBool("isPerformingAction", isPerformingAction);
        playerManager.applyRootMotion = applyRootMotion;
        playerManager.animator.CrossFade(targetAnim, 0.2f);
        playerManager.isPerformingAction = isPerformingAction;
        playerManager.canRotate = canRotate;
        playerManager.canMove = canMove;
    }

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
