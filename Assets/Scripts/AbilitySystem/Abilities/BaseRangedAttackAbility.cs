using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Ranged Attack")]
public class BaseRangedAttackAbility : Ability
{
    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;
    public AudioClip fireSound;
    public float force = 1000f;

    public override void InitializeShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
        if (!MatchesSlot(instance, shot.slot)) return;
        
        shot.projectilePrefab = projectilePrefab;
        shot.muzzleFlashPrefab = muzzleFlashPrefab;
        shot.fireSound = fireSound;
        shot.force = force;
        shot.energyCost = energyCost;
    }



    public override void OnUpdate(GameObject owner, AbilityInstance instance)
    {
    	var system = owner.GetComponent<AbilitySystem>();
        if (!system.IsHeld(instance.slot)) return;
        if (!instance.CanFire()) return;

        var energy = owner.GetComponent<Energy>();
        if (energy == null) return;
		
		ShotData shot = system.GetData<ShotData>(instance.slot);
		// shot.slot = instance.slot;
        // system.BuildShotDataForSlot(shot);
        
        if (!energy.HasEnough(shot.energyCost)) return;

        system.Fire(instance.slot);

        energy.Spend(shot.energyCost);
        
        instance.TriggerFireRate(shot.fireRate);
    }
    
    public override void ModifyProjectile(GameObject owner, AbilityInstance instance, ProjectileData data)
    {
        if (!MatchesSlot(instance, data.slot)) return;
		data.OnHit += (target, hitPoint, hitDir) =>
		{
			var health = target.GetComponentInParent<Health>();
			if (health != null)
				health.TakeDamage((int)data.damage, data.context.owner);
		};

    }
    
    
}