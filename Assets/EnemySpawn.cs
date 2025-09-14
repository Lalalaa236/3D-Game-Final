using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
public class EnemyNav : MonoBehaviour
{
    [Header("Prefabs & Count")]
    [Tooltip("Enemy prefabs to spawn (picked at random).")]
    public GameObject[] enemyPrefabs;

    [Tooltip("How many enemies to spawn on Start (default 100).")]
    public int initialCount = 100;

    [Header("Spawn Area")]
    [Tooltip("Optional. If set, spawns inside this BoxCollider's volume. Otherwise uses fallback radius around this object.")]
    public BoxCollider spawnVolume;

    [Tooltip("Used when spawnVolume is not set.")]
    public float fallbackRadius = 40f;

    [Header("NavMesh Sampling")]
    [Tooltip("Max distance from the random point to search for the NavMesh.")]
    public float sampleMaxDistance = 4f;

    [Tooltip("Attempts per enemy to find a valid NavMesh point.")]
    public int maxAttemptsPerSpawn = 20;

    [Tooltip("Keep enemies at least this far apart at spawn (0 = disabled).")]
    public float minSeparation = 1.0f;

    [Header("Ownership")]
    [Tooltip("Parent transform for spawned enemies (optional).")]
    public Transform spawnParent;

    [Tooltip("Optional: target to assign to enemies (requires enemy script has SetTarget(Transform)).")]
    public Transform playerTarget;

    [Header("Runtime")]
    [Tooltip("Spawn on Start.")]
    public bool spawnOnStart = true;

    private NavMeshSurface surface;
    private readonly List<Vector3> _spawnedPositions = new();

    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            Debug.LogWarning("[EnemyNav] No enemy prefabs assigned.");

        if (spawnParent == null)
            spawnParent = this.transform;
    }

    void Start()
    {
        surface.BuildNavMesh();

        if (spawnOnStart && initialCount > 0)
            SpawnEnemies(initialCount);
    }

    public void SpawnEnemies(int count)
    {
        int spawned = 0;
        _spawnedPositions.Clear();

        for (int i = 0; i < count; i++)
        {
            if (TryGetRandomNavmeshPoint(out Vector3 pos))
            {
                SpawnOne(pos);
                _spawnedPositions.Add(pos);
                spawned++;
            }
            else
            {
                Debug.LogWarning($"[EnemyNav] Could not find NavMesh point for enemy #{i + 1}");
            }
        }

        Debug.Log($"[EnemyNav] Spawned {spawned}/{count} enemies.");
    }

    public void ClearSpawns()
    {
        int n = 0;
        for (int i = spawnParent.childCount - 1; i >= 0; i--)
        {
            Transform c = spawnParent.GetChild(i);
            if (c != this.transform)
            {
                DestroyImmediate(c.gameObject);
                n++;
            }
        }
        _spawnedPositions.Clear();
        Debug.Log($"[EnemyNav] Cleared {n} spawned enemies.");
    }

    private void SpawnOne(Vector3 position)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject go = Instantiate(prefab, position, Quaternion.identity, spawnParent);

        if (go.TryGetComponent(out NavMeshAgent agent))
            agent.Warp(position);
        else
            go.transform.position = position;

        if (playerTarget && go.TryGetComponent<AITarget>(out var ai))
            ai.SetTarget(playerTarget);
    }

    private bool TryGetRandomNavmeshPoint(out Vector3 result)
    {
        // If a spawnVolume is set, keep the original bounded sampling.
        if (spawnVolume != null)
        {
            Bounds b = ResolveBounds();
            for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
            {
                Vector3 rnd = RandomPointInBounds(b, transform.position, fallbackRadius);

                if (NavMesh.SamplePosition(rnd, out NavMeshHit hit, sampleMaxDistance, NavMesh.AllAreas))
                {
                    if (minSeparation > 0f)
                    {
                        bool ok = true;
                        for (int i = 0; i < _spawnedPositions.Count; i++)
                        {
                            if ((hit.position - _spawnedPositions[i]).sqrMagnitude < (minSeparation * minSeparation))
                            {
                                ok = false; break;
                            }
                        }
                        if (!ok) continue;
                    }

                    result = hit.position;
                    return true;
                }
            }
            result = default;
            return false;
        }

        // No spawnVolume: sample across the whole NavMesh
        for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
        {
            if (RandomPointOnNavMesh(out Vector3 navPoint))
            {
                // Optional snap to navmesh to be safe
                if (NavMesh.SamplePosition(navPoint, out NavMeshHit hit, sampleMaxDistance, NavMesh.AllAreas))
                {
                    if (minSeparation > 0f)
                    {
                        bool ok = true;
                        for (int i = 0; i < _spawnedPositions.Count; i++)
                        {
                            if ((hit.position - _spawnedPositions[i]).sqrMagnitude < (minSeparation * minSeparation))
                            {
                                ok = false; break;
                            }
                        }
                        if (!ok) continue;
                    }

                    result = hit.position;
                    return true;
                }
            }
        }

        result = default;
        return false;
    }

    private Bounds ResolveBounds()
    {
        if (spawnVolume != null)
            return spawnVolume.bounds;

        var center = transform.position + Vector3.up * 1.0f;
        var size = new Vector3(fallbackRadius * 2f, 10f, fallbackRadius * 2f);
        return new Bounds(center, size);
    }

    private Vector3 RandomPointInBounds(Bounds b, Vector3 fallbackCenter, float radius)
    {
        if (spawnVolume != null)
        {
            return new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                Random.Range(b.min.z, b.max.z)
            );
        }
        else
        {
            // Flat disc around fallbackCenter (kept as fallback if needed)
            Vector2 circle = Random.insideUnitCircle * radius;
            return new Vector3(fallbackCenter.x + circle.x, b.center.y, fallbackCenter.z + circle.y);
        }
    }

    // New helper: picks a random point over the entire NavMesh surface,
    // sampling triangles weighted by area.
    private bool RandomPointOnNavMesh(out Vector3 point)
    {
        point = default;
        var triang = NavMesh.CalculateTriangulation();
        var vertices = triang.vertices;
        var indices = triang.indices;

        if (vertices == null || indices == null || indices.Length < 3)
            return false;

        int triCount = indices.Length / 3;
        // Build cumulative areas
        float totalArea = 0f;
        // Small allocation but acceptable for occasional spawning
        float[] areas = new float[triCount];
        for (int i = 0; i < triCount; i++)
        {
            Vector3 a = vertices[indices[i * 3]];
            Vector3 b = vertices[indices[i * 3 + 1]];
            Vector3 c = vertices[indices[i * 3 + 2]];
            float area = Vector3.Cross(b - a, c - a).magnitude * 0.5f;
            areas[i] = area;
            totalArea += area;
        }

        if (totalArea <= Mathf.Epsilon)
            return false;

        // Pick a triangle weighted by area
        float r = Random.value * totalArea;
        int chosenTri = 0;
        float accum = 0f;
        for (int i = 0; i < triCount; i++)
        {
            accum += areas[i];
            if (r <= accum)
            {
                chosenTri = i;
                break;
            }
        }

        Vector3 v0 = vertices[indices[chosenTri * 3]];
        Vector3 v1 = vertices[indices[chosenTri * 3 + 1]];
        Vector3 v2 = vertices[indices[chosenTri * 3 + 2]];

        // Random barycentric coordinates
        float u = Random.value;
        float v = Random.value;
        if (u + v > 1f) { u = 1f - u; v = 1f - v; }
        point = v0 + (v1 - v0) * u + (v2 - v0) * v;
        return true;
    }

    [ContextMenu("Spawn Now")]
    private void _ContextSpawnNow()
    {
        SpawnEnemies(initialCount);
    }

    [ContextMenu("Clear Spawns")]
    private void _ContextClearSpawns()
    {
        ClearSpawns();
    }
}
