using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeleeComboStep
{
    public string animatorTrigger = "Attack";
    public int damage = 20;
    public float energyCost = 10f;
    public float range = 2f;
    [Range(0f, 360f)]
    public float attackAngle = 90f;
    public GameObject slashPrefab;
    public float slashXPos = 0.5f;
    public float slashYPos = 0.5f;
    public float slashZPos = 0.5f;
    public float slashXRot = 90f;
    public float slashYRot = 0f;
    public float slashZRot = 0f;
    public float slashScale = 1f;
    public GameObject impactPrefab;
}

[CreateAssetMenu(menuName = "Abilities/Base Melee Attack")]
public class BaseMeleeAttackAbility : Ability
{
    public List<MeleeComboStep> comboSteps = new List<MeleeComboStep>();
    public float attackRate = 0.5f;
    public float comboResetTime = 1.5f;
    public LayerMask targetLayers;
    public AudioClip attackSound;

    [Tooltip("When enabled, hit detection waits for an animation event rather than firing immediately on key press.")]
    public bool useAnimationEvents = false;

    // Per-instance runtime state (SO is shared, so keyed by instance)
    private readonly Dictionary<AbilityInstance, int> comboIndices = new Dictionary<AbilityInstance, int>();
    private readonly Dictionary<AbilityInstance, float> comboTimers = new Dictionary<AbilityInstance, float>();
    private readonly Dictionary<AbilityInstance, MeleeComboStep> pendingSteps = new Dictionary<AbilityInstance, MeleeComboStep>();

    public override void OnEquip(GameObject owner, AbilityInstance instance)
    {
        comboIndices[instance] = 0;
        comboTimers[instance] = 0f;
    }

    public override void OnUnequip(GameObject owner, AbilityInstance instance)
    {
        comboIndices.Remove(instance);
        comboTimers.Remove(instance);
        pendingSteps.Remove(instance);
    }

    public override void InitializeMelee(GameObject owner, AbilityInstance instance, MeleeData melee)
    {
        if (!MatchesSlot(instance, melee.slot)) return;
        if (!pendingSteps.TryGetValue(instance, out var step)) return;

        melee.damage = step.damage;
        melee.range = step.range;
        melee.attackAngle = step.attackAngle;
        melee.energyCost = step.energyCost;
        melee.targetLayers = targetLayers;
        melee.impactPrefab = step.impactPrefab;
        melee.attackRate = attackRate;
    }

    public override void OnUpdate(GameObject owner, AbilityInstance instance)
    {
        
        var system = owner.GetComponent<AbilitySystem>();
        MeleeData data = system.GetData<MeleeData>(instance.slot);
        //MeleeData data = new MeleeData();
		//data.slot = instance.slot;
        //system.BuildMeleeDataForSlot(data);

        
        // Tick combo reset timer
        if (comboTimers.TryGetValue(instance, out float t))
        {
            t -= Time.deltaTime;
            comboTimers[instance] = t;
            if (t <= 0f)
                comboIndices[instance] = 0;
        }

        if (!system.IsHeld(instance.slot)) return;
        if (!instance.CanFire()) return;
        if (comboSteps.Count == 0) return;

        int idx = comboIndices.TryGetValue(instance, out int i) ? i : 0;
        var step = comboSteps[idx % comboSteps.Count];

        var energy = owner.GetComponent<Energy>();
        if (energy == null || !energy.HasEnough(step.energyCost)) return;

        energy.Spend(step.energyCost);
        instance.TriggerFireRate(data.attackRate);

        // Store step so InitializeMelee can read it
        pendingSteps[instance] = step;

        // Spawn slash effect
        if (step.slashPrefab != null)
        {
            Vector3 localOffset = new Vector3(step.slashXPos, step.slashYPos, step.slashZPos);
            Vector3 spawnPos = owner.transform.position + owner.transform.TransformDirection(localOffset);
            Quaternion spawnRot = owner.transform.rotation * Quaternion.Euler(step.slashXRot, step.slashYRot, step.slashZRot);
            var slash = UnityEngine.Object.Instantiate(step.slashPrefab, spawnPos, spawnRot);
            slash.transform.localScale = Vector3.one * step.slashScale;
        }

        if (!string.IsNullOrEmpty(step.animatorTrigger))
            owner.GetComponentInChildren<Animator>()?.SetTrigger(step.animatorTrigger);

        if (attackSound != null)
            AudioSource.PlayClipAtPoint(attackSound, owner.transform.position);

        // Advance combo, reset timer
        comboIndices[instance] = (idx + 1) % comboSteps.Count;
        comboTimers[instance] = comboResetTime;

        if (!useAnimationEvents)
            system.TriggerMeleeHit(instance.slot);
    }

    public override void OnMeleeHit(GameObject owner, AbilityInstance instance, MeleeData melee)
    {
        if (!MatchesSlot(instance, melee.slot)) return;

        var system = owner.GetComponent<AbilitySystem>();
        Transform origin = system.firePoint != null ? system.firePoint : owner.transform;

        LayerMask mask = melee.targetLayers.value == 0 ? Physics.DefaultRaycastLayers : melee.targetLayers;
        var hits = Physics.OverlapSphere(origin.position, melee.range, mask);

        foreach (var hit in hits)
        {
            if (hit.gameObject == owner) continue;

            Vector3 toTarget = (hit.transform.position - origin.position).normalized;
            if (Vector3.Angle(origin.forward, toTarget) > melee.attackAngle * 0.5f) continue;

            var health = hit.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage((int)melee.damage, melee.context?.owner);
                melee.OnHit?.Invoke(hit.gameObject);

                
            }
            if (melee.impactPrefab != null)
            {
                Vector3 impactPos = hit.ClosestPoint(origin.position);
                UnityEngine.Object.Instantiate(melee.impactPrefab, impactPos, Quaternion.LookRotation(-toTarget));
            }
        }
    }
}
