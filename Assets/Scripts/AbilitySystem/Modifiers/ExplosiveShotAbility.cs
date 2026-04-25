using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Explosive Shot")]
public class ExplosiveShotAbility : Ability
{
    public GameObject explosionPrefab;
    public float explosionRadius = 5f;
    public float explosionDamage = 50f;
    public LayerMask damageLayers;
    [Tooltip("Orient the explosion to face back toward the direction the projectile came from.")]
    public bool orientToImpact = false;
    [Tooltip("The local axis of the explosion prefab that should point back toward the attacker. Y for effects that emit upward, Z for effects that emit forward.")]
    public Vector3 effectOriginAxis = Vector3.up;
    
    
    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData data)
    {
    	if (!MatchesSlot(instance, data.slot)) return;
    	
        data.energyCost += energyCost;


    }

    public override void ModifyProjectile(GameObject owner, AbilityInstance instance, ProjectileData data)
    {
        
        
        if (!MatchesSlot(instance, data.slot)) return;
		data.OnHit += (target, hitPoint, hitDir) =>
		{
		
			if (explosionPrefab != null)
			{
				Quaternion rot = orientToImpact && hitDir != Vector3.zero
					? Quaternion.FromToRotation(effectOriginAxis, -hitDir)
					: Quaternion.identity;

				GameObject obj = Instantiate(explosionPrefab, hitPoint, rot);
				obj.tag = "FX";

				// Force local simulation space so particle systems respect the rotation
				if (orientToImpact)
				{
					foreach (var ps in obj.GetComponentsInChildren<ParticleSystem>())
					{
						var main = ps.main;
						main.simulationSpace = ParticleSystemSimulationSpace.Local;
					}
				}
			}

			var hits = Physics.OverlapSphere(hitPoint, explosionRadius, damageLayers);
			foreach (var hit in hits)
			{
				if (hit.gameObject == target) continue;

				var health = hit.GetComponentInParent<Health>();
				if (health != null)
					health.TakeDamage(explosionDamage);
			}
		};

        
        
        
    }
}