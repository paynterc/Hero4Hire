using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Ranged Attack")]
public class BaseRangedAttackAbility : Ability
{
    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;
    public AudioClip fireSound;

    public float force = 1000f;

    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
        if (!MatchesSlot(instance, shot.slot)) return;
        
        shot.projectilePrefab = projectilePrefab;
        shot.muzzleFlashPrefab = muzzleFlashPrefab;
        shot.fireSound = fireSound;
        shot.force = force;
        shot.energyCost += energyCost;
    }

    public override void OnUpdate(GameObject owner, AbilityInstance instance)
    {
    	var system = owner.GetComponent<AbilitySystem>();
        if (!system.IsHeld(instance.slot)) return;
        if (!instance.CanFire()) return;

        var energy = owner.GetComponent<Energy>();
        if (energy == null) return;

        float totalCost = system.CalculateShotCost(instance.slot);
        if (!energy.HasEnough(totalCost)) return;

        system.Fire(instance.slot);

        energy.Spend(totalCost);
        instance.TriggerFireRate(0.2f);
    }
    
    public override void ModifyProjectile(GameObject owner, AbilityInstance instance, ProjectileData data)
    {
        if (!MatchesSlot(instance, data.slot)) return;
		data.OnHit += (target) =>
		{

			var health = target.GetComponent<Health>();
			if (health != null)
			{
				health.TakeDamage((int)data.damage);
			}
		};

    }
    
    
}