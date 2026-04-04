using UnityEngine;

public class ExitZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other) => TryTrigger(other);
    void OnTriggerStay(Collider other) => TryTrigger(other);

    void TryTrigger(Collider other)
    {
        other.GetComponentInParent<DestroyOnTouch>()?.Trigger();
    }
}
