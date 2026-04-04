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
        {
            	//Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            Collider col = GetComponent<Collider>();
			if (col != null)
			{
				Vector3 pos = new Vector3(
					col.bounds.center.x,
					col.bounds.min.y,
					col.bounds.center.z
				);

				GameObject obj = Instantiate(explosionPrefab, pos, Quaternion.identity);
				obj.tag = "FX";
			}
        
        }

            

        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.SetTrigger(deathAnimationTrigger);

        var player = GetComponent<PlayerController>();
        if (player != null) player.enabled = false;

        var ai = GetComponent<AIBrain>();
        if (ai != null)
        {
            ai.DropCarried();
            ai.enabled = false;
        }

        var abilities = GetComponent<AbilitySystem>();
        if (abilities != null) abilities.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}
