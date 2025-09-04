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
}
