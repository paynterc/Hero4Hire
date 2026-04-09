using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class IntervalSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;
    public float spawnInterval = 2f;
    public int unitsPerSpawn = 3;

    [Header("Spawn Area (centered on spawner)")]
    public Vector3 spawnAreaSize = new Vector3(5f, 2f, 5f);

    [Header("Rotation")]
    public Vector3 spawnRotationEuler = Vector3.zero;

    [Header("Placement Mode")]
    public bool useNavMesh = true;
    public float fixedYLevel = 0f;

    [Header("NavMesh")]
    public float navMeshSearchRadius = 5f;

    [Header("Population Control")]
    public LayerMask targetLayers; // supports multiple layers
    public int maxAllowed = 20;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            while (GetCountOnLayers() >= maxAllowed)
            {
                yield return new WaitForSeconds(0.5f);
            }

            // Wait BEFORE spawning
        	yield return new WaitForSeconds(spawnInterval);

        	SpawnBatch();
        }
    }

    int GetCountOnLayers()
    {
        int count = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (IsInLayerMask(obj.layer, targetLayers))
            {
                count++;
            }
        }

        return count;
    }

    bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    void SpawnBatch()
    {
        for (int i = 0; i < unitsPerSpawn; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                spawnAreaSize.y,
                Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
            );

            Vector3 worldPos = transform.position + randomOffset;
            Quaternion rotation = Quaternion.Euler(spawnRotationEuler);

            if (useNavMesh)
            {
                if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
                {
                    Vector3 finalPos = hit.position;
                    finalPos.y = fixedYLevel;
                    Instantiate(prefab, finalPos, rotation);
                }
            }
            else
            {
                worldPos.y = fixedYLevel;
                Instantiate(prefab, worldPos, rotation);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 center = transform.position + new Vector3(0, spawnAreaSize.y / 2f, 0);
        Gizmos.DrawWireCube(center, spawnAreaSize);
    }
}
