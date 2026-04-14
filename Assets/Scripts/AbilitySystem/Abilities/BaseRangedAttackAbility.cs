using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Ranged Attack")]
public class BaseRangedAttackAbility : Ability
{
    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;
    public AudioClip fireSound;
    public float force = 1000f;
    public float windupTime = 0f;
    public string windupAnimatorTrigger = "";

    private readonly Dictionary<AbilityInstance, float> windupTimers = new Dictionary<AbilityInstance, float>();
    private readonly Dictionary<AbilityInstance, bool>  wasHeld      = new Dictionary<AbilityInstance, bool>();

    public override void OnEquip(GameObject owner, AbilityInstance instance)
    {
        windupTimers[instance] = -1f;
        wasHeld[instance]      = false;
    }

    public override void OnUnequip(GameObject owner, AbilityInstance instance)
    {
        windupTimers.Remove(instance);
        wasHeld.Remove(instance);
    }

    public override void InitializeShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
        if (!MatchesSlot(instance, shot.slot)) return;

        shot.projectilePrefab        = projectilePrefab;
        shot.muzzleFlashPrefab       = muzzleFlashPrefab;
        shot.fireSound               = fireSound;
        shot.force                   = force;
        shot.energyCost              = energyCost;
        shot.windupTime              = windupTime;
        shot.windupAnimatorTrigger   = windupAnimatorTrigger;
    }

    public override void OnUpdate(GameObject owner, AbilityInstance instance)
    {
        var system = owner.GetComponent<AbilitySystem>();
        bool held = system.IsHeld(instance.slot);
        bool prev = wasHeld.TryGetValue(instance, out bool wh) ? wh : false;
        wasHeld[instance] = held;

        if (!held)
        {
            windupTimers[instance] = -1f;
            return;
        }

        ShotData shot = system.GetData<ShotData>(instance.slot);

        // Transition: not-held → held — start windup if configured
        if (!prev && shot.windupTime > 0f)
        {
            windupTimers[instance] = shot.windupTime;
            if (!string.IsNullOrEmpty(shot.windupAnimatorTrigger))
                owner.GetComponentInChildren<Animator>()?.SetTrigger(shot.windupAnimatorTrigger);
            return;
        }

        // Still winding up — tick timer
        if (windupTimers.TryGetValue(instance, out float remaining) && remaining > 0f)
        {
            windupTimers[instance] = remaining - Time.deltaTime;
            return;
        }

        // Windup complete (or no windup) — fire
        if (!instance.CanFire()) return;

        var energy = owner.GetComponent<Energy>();
        if (energy == null || !energy.HasEnough(shot.energyCost)) return;

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
