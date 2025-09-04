using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRollSFX()
    {
        audioSource.PlayOneShot(WorldSFXManager.instance.rollSFX);
    }
}
