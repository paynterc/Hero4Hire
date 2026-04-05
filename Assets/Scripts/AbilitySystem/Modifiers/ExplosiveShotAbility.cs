using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Explosive Shot")]
public class ExplosiveShotAbility : Ability
{
    public GameObject explosionPrefab;
    public float explosionRadius = 5f;
    public int explosionDamage = 50;
    public LayerMask damageLayers;
    
    
    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData data)
    {
    	if (!MatchesSlot(instance, data.slot)) return;
    	
        data.energyCost += energyCost;


    }

    public override void ModifyProjectile(GameObject owner, AbilityInstance instance, ProjectileData data)
    {
        
        
        if (!MatchesSlot(instance, data.slot)) return;
		data.OnHit += (owner) =>
		{


			Vector3 position = owner.transform.position;

			if (explosionPrefab != null)
				Instantiate(explosionPrefab, position, Quaternion.identity);


			var hits = Physics.OverlapSphere(position, explosionRadius, damageLayers);
			foreach (var hit in hits)
			{
				// Debug.Log("Start Explosion Hits");
				if (hit.gameObject == owner) continue;

				var health = hit.GetComponent<Health>();
				if (health != null)
					health.TakeDamage(explosionDamage);
			}

		};

        
        
        
    }
}