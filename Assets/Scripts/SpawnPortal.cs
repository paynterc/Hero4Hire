using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SpawnPortal : MonoBehaviour
{
    [Header("Scaling")]
    public Vector3 startScale = Vector3.zero;
    public Vector3 endScale = Vector3.one;
    public float growDuration = 1f;
    public float shrinkDuration = 1f;

    [Header("Timing")]
    public float waitBeforeSpawn = 1f;
    public float waitBeforeShrink = 2f;

    [Header("Spawning")]
    public GameObject prefab;
    public int spawnCount = 5;
    public Vector3 spawnAreaSize = new Vector3(5f, 2f, 5f); // width, height, depth
    public float forwardOffset = 3f;

    private void Start()
    {
        transform.localScale = startScale;
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        yield return ScaleOverTime(startScale, endScale, growDuration);

        yield return new WaitForSeconds(waitBeforeSpawn);

        SpawnPrefabs();

        yield return new WaitForSeconds(waitBeforeShrink);

        yield return ScaleOverTime(endScale, startScale, shrinkDuration);

        Destroy(gameObject);
    }

    IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = to;
    }

    void SpawnPrefabs()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomLocal = new Vector3(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                spawnAreaSize.y,
                Random.Range(0, spawnAreaSize.z)
            );

            Vector3 worldPos = transform.TransformPoint(randomLocal + Vector3.forward * forwardOffset);

            if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                Instantiate(prefab, hit.position, Quaternion.identity);
            }
        }
    }
}
