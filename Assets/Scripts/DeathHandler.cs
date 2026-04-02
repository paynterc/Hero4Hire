using UnityEngine;

public class DeathHandler : MonoBehaviour
{
    public GameObject explosionPrefab;
    public string deathAnimationTrigger = "Death";
    public float destroyDelay = 3f;

    void Start()
    {
        GetComponent<Health>().OnDeath += HandleDeath;
    }

    void HandleDeath()
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger(deathAnimationTrigger);

        var player = GetComponent<PlayerController>();
        if (player != null) player.enabled = false;

        var ai = GetComponent<AIBrain>();
        if (ai != null) ai.enabled = false;

        var abilities = GetComponent<AbilitySystem>();
        if (abilities != null) abilities.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}
