using UnityEngine;

public enum AbilityType
{
    AttackBase,
    AttackModifier,
    MeleeBase,
    MeleeModifier,
    ShieldBase,
    ShieldModifier,
    DashBase,
    DashModifier,
    JumpBase,
    JumpModifier,
    Passive
}

public enum ActionSlot
{
    Primary,
    Secondary,
    Jump,
    Dash,
    Ultimate,
    Action1,
    Action2,
    Action3
}


public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public AbilityType abilityType;
	public float energyCost = 20f;
    public bool disableIK = false;

    public virtual void OnEquip(GameObject owner, AbilityInstance instance) { }
    public virtual void OnUnequip(GameObject owner, AbilityInstance instance) { }

    public virtual void OnUpdate(GameObject owner, AbilityInstance instance) { }

    public virtual void OnDash(GameObject owner, AbilityInstance instance, DashData dash) { }
    public virtual void OnJump(GameObject owner, AbilityInstance instance, JumpData jump) { }
    public virtual void OnLand(GameObject owner, AbilityInstance instance, JumpData jump) {}


    public virtual void InitializeShot(GameObject owner, AbilityInstance instance, ShotData shot) { }
    public virtual void InitializeDash(GameObject owner, AbilityInstance instance, DashData dash) { }
    public virtual void InitializeJump(GameObject owner, AbilityInstance instance, JumpData jump) { }
    
    
    public virtual void ModifyShot(GameObject owner, AbilityInstance instance, ShotData shot) { }
    public virtual void ModifyDash(GameObject owner, AbilityInstance instance, DashData dash) { }
    public virtual void ModifyJump(GameObject owner, AbilityInstance instance, JumpData jump) { }
    
    
    public virtual void ModifyAction(GameObject owner, AbilityInstance instance, IActionData action)
	{
		if (action is ShotData shot)
			ModifyShot(owner, instance, shot);

		if (action is DashData dash)
			ModifyDash(owner, instance, dash);
	}

	protected bool MatchesSlot(AbilityInstance instance, ActionSlot actionSlot)
    {
        return instance.slot == actionSlot;
    }

	public virtual void ModifyProjectile(
        GameObject owner,
        AbilityInstance instance,
        ProjectileData projectile
    ) { }

    public virtual void InitializeMelee(GameObject owner, AbilityInstance instance, MeleeData melee) { }
    public virtual void ModifyMelee(GameObject owner, AbilityInstance instance, MeleeData melee) { }
    public virtual void OnMeleeHit(GameObject owner, AbilityInstance instance, MeleeData melee) { }

    public virtual void InitializeShield(GameObject owner, AbilityInstance instance, ShieldData data) { }
    public virtual void ModifyShield(GameObject owner, AbilityInstance instance, ShieldData data) { }
    public virtual void OnActivateShield(GameObject owner, AbilityInstance instance, ShieldInstance shield) { }
}
