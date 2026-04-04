using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Landing Explosion")]
public class LandingExplosionAbility : Ability
{
    public GameObject explosionPrefab;
    public float explosionRadius = 5f;
    public int explosionDamage = 50;
    public LayerMask damageLayers;
    
    
    public override void ModifyJump(GameObject owner, AbilityInstance instance, JumpData jump)
    {
    	if (!MatchesSlot(instance, jump.slot)) return;
    	
        jump.energyCost += energyCost;


    }

    public override void OnLand(GameObject owner, AbilityInstance instance, JumpData jump)
    {
        Vector3 position = owner.transform.position;

        if (explosionPrefab != null)
        {
        	GameObject obj = Instantiate(explosionPrefab, position, Quaternion.identity);
			obj.tag = "FX";
        }
            

        var hits = Physics.OverlapSphere(position, explosionRadius, damageLayers);
        foreach (var hit in hits)
        {
            if (hit.gameObject == owner) continue;

            var health = hit.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(explosionDamage);
        }
    }
}
