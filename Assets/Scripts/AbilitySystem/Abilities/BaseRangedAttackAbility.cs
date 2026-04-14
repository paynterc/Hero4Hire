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
    public GameObject windupPrefab;

    private readonly Dictionary<AbilityInstance, float>      windupTimers    = new Dictionary<AbilityInstance, float>();
    private readonly Dictionary<AbilityInstance, bool>       wasHeld         = new Dictionary<AbilityInstance, bool>();
    private readonly Dictionary<AbilityInstance, GameObject> windupInstances = new Dictionary<AbilityInstance, GameObject>();

    public override void OnEquip(GameObject owner, AbilityInstance instance)
    {
        windupTimers[instance]    = -1f;
        wasHeld[instance]         = false;
        windupInstances[instance] = null;
    }

    public override void OnUnequip(GameObject owner, AbilityInstance instance)
    {
        DestroyWindupEffect(instance);
        windupTimers.Remove(instance);
        wasHeld.Remove(instance);
        windupInstances.Remove(instance);
    }

    public override void InitializeShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
        if (!MatchesSlot(instance, shot.slot)) return;

        shot.projectilePrefab       = projectilePrefab;
        shot.muzzleFlashPrefab      = muzzleFlashPrefab;
        shot.fireSound              = fireSound;
        shot.force                  = force;
        shot.energyCost             = energyCost;
        shot.windupTime             = windupTime;
        shot.windupAnimatorTrigger  = windupAnimatorTrigger;
        shot.windupPrefab           = windupPrefab;
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
            DestroyWindupEffect(instance);
            return;
        }

        ShotData shot = system.GetData<ShotData>(instance.slot);

        // Transition: not-held → held — start windup if configured
        if (!prev && shot.windupTime > 0f)
        {
            windupTimers[instance] = shot.windupTime;

            if (!string.IsNullOrEmpty(shot.windupAnimatorTrigger))
                owner.GetComponentInChildren<Animator>()?.SetTrigger(shot.windupAnimatorTrigger);

            if (shot.windupPrefab != null)
            {
                Transform spawnPoint = system.firePoint != null ? system.firePoint : owner.transform;
                var fx = Instantiate(shot.windupPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
                fx.transform.localScale = Vector3.one * 0.25f;
                windupInstances[instance] = fx;
            }
            return;
        }

        // Still winding up — scale effect and tick timer
        if (windupTimers.TryGetValue(instance, out float remaining) && remaining > 0f)
        {
            if (windupInstances.TryGetValue(instance, out var fx) && fx != null)
            {
                float progress = 1f - (remaining / shot.windupTime);
                fx.transform.localScale = Vector3.one * Mathf.Lerp(0.25f, 1f, progress);
            }
            windupTimers[instance] = remaining - Time.deltaTime;
            return;
        }

        // Windup complete — destroy effect and fire
        DestroyWindupEffect(instance);

        if (!instance.CanFire()) return;

        var energy = owner.GetComponent<Energy>();
        if (energy == null || !energy.HasEnough(shot.energyCost)) return;

        system.Fire(instance.slot);
        energy.Spend(shot.energyCost);
        instance.TriggerFireRate(shot.fireRate);
    }

    void DestroyWindupEffect(AbilityInstance instance)
    {
        if (windupInstances.TryGetValue(instance, out var fx) && fx != null)
            Destroy(fx);
        windupInstances[instance] = null;
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
