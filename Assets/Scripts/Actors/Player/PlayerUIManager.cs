using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    public PlayerHUDManager playerHUDManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // playerHUDManager = GetComponent<PlayerHUDManager>();
        DontDestroyOnLoad(gameObject);
    }
}
