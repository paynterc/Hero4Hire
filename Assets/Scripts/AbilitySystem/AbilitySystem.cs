using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct AbilitySlotBinding
{
    public Ability ability;
    public ActionSlot slot;
}

public class AbilitySystem : MonoBehaviour
{
    public List<AbilitySlotBinding> startingAbilities;
    public Transform firePoint;

    private List<AbilityInstance> abilities = new List<AbilityInstance>();
    
    private Dictionary<ActionSlot, bool> heldInputs = new Dictionary<ActionSlot, bool>();


    void Start()
    {
        foreach (var binding in startingAbilities)
        {
            var instance = new AbilityInstance(binding.ability, binding.slot);
            abilities.Add(instance);
            binding.ability.OnEquip(gameObject, instance);
        }
    }
    
    public void SetHeld(ActionSlot slot, bool isHeld)
	{
		heldInputs[slot] = isHeld;
	}
	
	public bool IsHeld(ActionSlot slot)
	{
		return heldInputs.TryGetValue(slot, out var held) && held;
	}



    void Update()
    {
        foreach (var ability in abilities)
        {
            ability.Update(gameObject);
        }
    }


    public void TriggerDash(ActionSlot slot)
    {
    
        var dash = new DashData{slot=slot};
        
        foreach (var ability in abilities)
		{
			ability.ability.InitializeDash(gameObject, ability, dash);
		}
        
        
        // Let ALL abilities modify the dash
		foreach (var ability in abilities)
		{
			ability.ability.ModifyDash(gameObject, ability, dash);
		}

    
        foreach (var ability in abilities)
        {
            ability.ability.OnDash(gameObject, ability, dash);
        }
        
        
    }
    
    public void TriggerJump(ActionSlot slot)
    {
        var jump = new JumpData{slot=slot};
        
        foreach (var ability in abilities)
		{
			ability.ability.InitializeJump(gameObject, ability, jump);
		}
        
        // Let ALL abilities modify the jump
		foreach (var ability in abilities)
		{
			ability.ability.ModifyJump(gameObject, ability, jump);
		}
    
        foreach (var ability in abilities)
        {
            ability.ability.OnJump(gameObject, ability, jump);
        }
    }
    
    

    public T GetComponentFromOwner<T>() where T : Component
    {
        return GetComponent<T>();
    }
    
    public void Fire(ActionSlot slot)
	{
		ShotData shot = new ShotData();

		Transform origin = firePoint != null ? firePoint : transform;
		shot.origin = origin.position;
		shot.direction = origin.forward;
		shot.slot = slot;
		shot.context = new ActionContext
		{
			owner = gameObject,
			timestamp = Time.time
		};

		// Let ALL abilities modify the shot
		foreach (var ability in abilities)
		{
			ability.ability.ModifyAction(gameObject, ability, shot);
		}

		// Execute final shot
		ExecuteShot(shot);
	}
	
	void ExecuteShot(ShotData shot)
	{
		// Play sound
		if (shot.fireSound != null)
		{
		AudioSource.PlayClipAtPoint(shot.fireSound, shot.origin);
		}

		// Muzzle flash
		if (shot.muzzleFlashPrefab != null)
		{
			Instantiate(
				shot.muzzleFlashPrefab,
				shot.origin,
				Quaternion.LookRotation(shot.direction)
			);
		}

		// Spawn projectiles
		for (int i = 0; i < shot.projectileCount; i++)
		{
			// Apply spread
			Quaternion spreadRot = Quaternion.Euler(
				Random.Range(-shot.spreadAngle, shot.spreadAngle),
				Random.Range(-shot.spreadAngle, shot.spreadAngle),
				0f
			);

			Vector3 dir = spreadRot * shot.direction;

			// Instantiate projectile
			GameObject proj = Instantiate(
				shot.projectilePrefab,
				shot.origin,
				Quaternion.LookRotation(dir)
			);

			// Build ProjectileData
			ProjectileData projectileData = new ProjectileData
			{
				speed = shot.force,
				lifetime = 3f,
				damage = shot.damage, // adjust later if needed
				context = shot.context,

				OnSpawn = (go) =>
				{
					// Optional hook
				},

				OnHit = (target) =>
				{
					// Future: apply damage here
				},
				slot = shot.slot
			};

			// Let abilities modify projectile behavior
			foreach (var ability in abilities)
			{
				ability.ability.ModifyProjectile(gameObject, ability, projectileData);
			}

			// Attach AbilityProjectile (runtime)
			var abilityProj = proj.GetComponent<AbilityProjectile>();
			if (abilityProj == null)
			{
				abilityProj = proj.AddComponent<AbilityProjectile>();
			}

			abilityProj.Init(projectileData);

			// Apply physics
			Rigidbody rb = proj.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.linearVelocity = Vector3.zero;
				rb.AddForce(dir * shot.force);
			}
		}

	}

	
	public float CalculateShotCost(ActionSlot slot)
	{
		var shot = new ShotData { slot = slot };
		foreach (var ability in abilities)
			ability.ability.ModifyShot(gameObject, ability, shot);
		return shot.energyCost;
	}

	public void TriggerLand(ActionSlot slot)
	{
		var jump = new JumpData { slot = slot };

		foreach (var ability in abilities)
		{
			ability.ability.OnLand(gameObject, ability, jump);
		}
	}

	bool HasBaseInSlot(ActionSlot slot, AbilityType type)
	{
		return abilities.Any(a =>
			a.slot == slot &&
			a.ability.abilityType == type);
	}



}
