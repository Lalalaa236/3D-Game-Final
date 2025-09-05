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
        if (playerManager.applyRootMotion)
        {
            Vector3 deltaPosition = playerManager.animator.deltaPosition;
            playerManager.characterController.Move(deltaPosition);
            playerManager.transform.rotation *= playerManager.animator.deltaRotation;
        }        
    }
}
