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

    private Dictionary<(System.Type, ActionSlot), (object data, int frame)> dataCache
        = new Dictionary<(System.Type, ActionSlot), (object data, int frame)>();
    private Dictionary<System.Type, System.Func<ActionSlot, object>> dataBuilders
        = new Dictionary<System.Type, System.Func<ActionSlot, object>>();

    void Start()
    {
        foreach (var binding in startingAbilities)
        {
            var instance = new AbilityInstance(binding.ability, binding.slot);
            abilities.Add(instance);
            binding.ability.OnEquip(gameObject, instance);
        }

        RegisterBuilder<ShotData>(slot =>
        {
            var d = new ShotData { slot = slot, context = new ActionContext { owner = gameObject } };
            foreach (var a in abilities) a.ability.InitializeShot(gameObject, a, d);
            foreach (var a in abilities) a.ability.ModifyShot(gameObject, a, d);
            return d;
        });

        RegisterBuilder<MeleeData>(slot =>
        {
            var d = new MeleeData { slot = slot, context = new ActionContext { owner = gameObject } };
            foreach (var a in abilities) a.ability.InitializeMelee(gameObject, a, d);
            foreach (var a in abilities) a.ability.ModifyMelee(gameObject, a, d);
            return d;
        });

        RegisterBuilder<JumpData>(slot =>
        {
            var d = new JumpData { slot = slot };
            foreach (var a in abilities) a.ability.InitializeJump(gameObject, a, d);
            foreach (var a in abilities) a.ability.ModifyJump(gameObject, a, d);
            return d;
        });

        RegisterBuilder<DashData>(slot =>
        {
            var d = new DashData { slot = slot };
            foreach (var a in abilities) a.ability.InitializeDash(gameObject, a, d);
            foreach (var a in abilities) a.ability.ModifyDash(gameObject, a, d);
            return d;
        });

        RegisterBuilder<ShieldData>(slot =>
        {
            var d = new ShieldData { slot = slot, context = new ActionContext { owner = gameObject } };
            foreach (var a in abilities) a.ability.InitializeShield(gameObject, a, d);
            foreach (var a in abilities) a.ability.ModifyShield(gameObject, a, d);
            return d;
        });
    }

    public void RegisterBuilder<T>(System.Func<ActionSlot, T> builder) where T : class
    {
        dataBuilders[typeof(T)] = slot => builder(slot);
    }

    public T GetData<T>(ActionSlot slot) where T : class
    {
        var key = (typeof(T), slot);
        if (dataCache.TryGetValue(key, out var cached) && cached.frame == Time.frameCount)
            return (T)cached.data;

        if (!dataBuilders.TryGetValue(typeof(T), out var build))
        {
            Debug.LogWarning($"[AbilitySystem] No builder registered for {typeof(T).Name}");
            return null;
        }

        var data = build(slot);
        dataCache[key] = (data, Time.frameCount);
        return (T)data;
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

        foreach (var ability in abilities)
		{
			ability.ability.InitializeShot(gameObject, ability, shot);
		}
        

		// Let ALL abilities modify the shot
		foreach (var ability in abilities)
		{
			ability.ability.ModifyShot(gameObject, ability, shot);
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

				OnHit = (target, hitPoint, hitDir) =>
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

	// ShotData should have the slot already added before you run these modifiers
	public void BuildShotDataForSlot(ShotData shot)
	{
	
		foreach (var ability in abilities)
			ability.ability.InitializeShot(gameObject, ability, shot);		
		foreach (var ability in abilities)
			ability.ability.ModifyShot(gameObject, ability, shot);
	
	}
	
	// ShotData should have the slot already added before you run these modifiers
	public void BuildMeleeDataForSlot(MeleeData data)
	{
	
		foreach (var ability in abilities)
			ability.ability.InitializeMelee(gameObject, ability, data);		
		foreach (var ability in abilities)
			ability.ability.ModifyMelee(gameObject, ability, data);
	
	}
	
		
	public float CalculateShotCost(ActionSlot slot)
	{
		return GetData<ShotData>(slot)?.energyCost ?? 0f;
	}
	

	

	public void TriggerLand(ActionSlot slot)
	{
		var jump = new JumpData { slot = slot };

		foreach (var ability in abilities)
		{
			ability.ability.OnLand(gameObject, ability, jump);
		}
	}

	public bool IsIKActive()
	{
		bool anyHeld = false;
		foreach (var ability in abilities)
		{
			if (IsHeld(ability.slot))
			{
				if (ability.ability.disableIK) return false;
				anyHeld = true;
			}
		}
		return anyHeld;
	}

	public void TriggerShield(ActionSlot slot)
	{
		var data = new ShieldData
		{
			slot = slot,
			context = new ActionContext { owner = gameObject, timestamp = Time.time }
		};

		foreach (var ability in abilities)
			ability.ability.InitializeShield(gameObject, ability, data);
		foreach (var ability in abilities)
			ability.ability.ModifyShield(gameObject, ability, data);

		if (data.shieldPrefab == null) return;

		// Destroy any existing shield first
		var existing = GetComponentInChildren<ShieldInstance>();
		if (existing != null)
			Destroy(existing.gameObject);

		var shieldObj = Instantiate(data.shieldPrefab, transform.position + Vector3.up * data.yOffset, Quaternion.identity, transform);
		shieldObj.transform.localScale = Vector3.one * data.radius;

		var shield = shieldObj.GetComponent<ShieldInstance>();
		if (shield == null)
			shield = shieldObj.AddComponent<ShieldInstance>();

		shield.Initialize(data, gameObject);

		foreach (var ability in abilities)
			ability.ability.OnActivateShield(gameObject, ability, shield);
	}

	public void TriggerMeleeHit(ActionSlot slot)
	{
		var melee = new MeleeData
		{
			slot = slot,
			context = new ActionContext { owner = gameObject, timestamp = Time.time }
		};

		foreach (var ability in abilities)
			ability.ability.InitializeMelee(gameObject, ability, melee);
		foreach (var ability in abilities)
			ability.ability.ModifyMelee(gameObject, ability, melee);
		foreach (var ability in abilities)
			ability.ability.OnMeleeHit(gameObject, ability, melee);
	}

	bool HasBaseInSlot(ActionSlot slot, AbilityType type)
	{
		return abilities.Any(a =>
			a.slot == slot &&
			a.ability.abilityType == type);
	}



}
