using UnityEngine;
using System.Collections;

public class DestroyOnTouch : MonoBehaviour
{
    public float delay = 3f; // time before destroy
    public bool started = false;
    public string targetTag;
    public LayerMask triggerLayer;

    bool IsValidTrigger(GameObject go)
    {
        if (triggerLayer.value != 0 && (triggerLayer.value & (1 << go.layer)) == 0) return false;
        if (!string.IsNullOrEmpty(targetTag) && !go.CompareTag(targetTag)) return false;
        return true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!started && IsValidTrigger(collision.gameObject))
        {
            started = true;
            StartCoroutine(DestroyAfterTime());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!started && IsValidTrigger(other.gameObject))
        {
            started = true;
            StartCoroutine(DestroyAfterTime());
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!started && IsValidTrigger(other.gameObject))
        {
            started = true;
            StartCoroutine(DestroyAfterTime());
        }
    }


    public void Trigger()
    {
        if (started) return;
        started = true;
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {

        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
