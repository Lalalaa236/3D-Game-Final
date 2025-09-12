using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Debug")]
    [SerializeField] private bool despawnCharacters = false;
    [SerializeField] private bool respawnCharacters = false;

    [Header("AI Characters")]
    [SerializeField] private GameObject[] aiCharacters;
    [SerializeField] private List<GameObject> spawnedInCharacters;

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnOnSceneLoaded = true;
    [SerializeField] private bool preventDuplicateSpawns = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (spawnOnSceneLoaded)
            SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (!spawnOnSceneLoaded)
            StartCoroutine(WaitForSceneToLoadThenSpawnCharacter());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SpawnNextFrame());
    }

    private IEnumerator SpawnNextFrame()
    {
        yield return null; // Wait for one frame to ensure the scene is fully loaded
        CleanSpawnList();
        SpawnAllCharacters();
    }

    private IEnumerator WaitForSceneToLoadThenSpawnCharacter()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }


        SpawnAllCharacters();
    }

    private void Update()
    {
        if (respawnCharacters)
        {
            respawnCharacters = false;
            DespawnAllCharacters();
            SpawnAllCharacters();
        }
        if (despawnCharacters)
        {
            despawnCharacters = false;
            DespawnAllCharacters();
        }
    }

    private void CleanSpawnList()
    {
        spawnedInCharacters.RemoveAll(item => item == null);
    }

    private void SpawnAllCharacters()
    {
        foreach (GameObject character in aiCharacters)
        {
            if (character != null)
            {
                if (preventDuplicateSpawns && spawnedInCharacters.Contains(character))
                    continue;

                GameObject newCharacter = Instantiate(character);
                spawnedInCharacters.Add(newCharacter);
            }
        }
    }

    public void DespawnAllCharacters()
    {
        foreach (GameObject character in spawnedInCharacters)
        {
            if (character != null)
            {
                Destroy(character);
            }
        }
        spawnedInCharacters.Clear();
    }
}