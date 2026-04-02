using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;
    public float spawnInterval = 1f;
    public int maxActive = 10;

    [Header("Optional")]
    public Transform spawnPoint;

    private List<GameObject> activeObjects = new List<GameObject>();
    private Coroutine spawnRoutine;

    void Start()
    {
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            CleanupList();

            if (activeObjects.Count < maxActive)
            {
                Spawn();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void Spawn()
    {
        Vector3 position = spawnPoint ? spawnPoint.position : transform.position;
        Quaternion rotation = spawnPoint ? spawnPoint.rotation : transform.rotation;

        GameObject obj = Instantiate(prefab, position, rotation);
        activeObjects.Add(obj);
    }

    void CleanupList()
    {
        // Remove destroyed objects from the list
        activeObjects.RemoveAll(obj => obj == null);
    }

    // Optional: manually stop spawning
    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }
    }

    // Optional: manually start spawning
    public void StartSpawning()
    {
        if (spawnRoutine == null)
        {
            spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }
}
