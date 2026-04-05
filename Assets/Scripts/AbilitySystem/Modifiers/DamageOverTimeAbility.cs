using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Damage Over Time")]
public class DamageOverTimeAbility : Ability
{
    public int damagePerTick = 5;
    public float tickInterval = 1f;
    public float duration = 5f;
    [Tooltip("If false, re-hitting the same target resets the timer instead of adding a new stack.")]
    public bool stackable = false;
    public GameObject effectPrefab;

    void ApplyDoT(GameObject target, GameObject attacker)
    {
        var health = target.GetComponent<Health>() ?? target.GetComponentInParent<Health>();
        if (health == null) return;

        var existing = health.GetComponent<DamageOverTime>();
        if (existing != null && !stackable)
        {
            existing.Refresh(duration);
            return;
        }

        var dot = health.gameObject.AddComponent<DamageOverTime>();
        dot.damagePerTick = damagePerTick;
        dot.tickInterval = tickInterval;
        dot.duration = duration;
        dot.attacker = attacker;
        dot.health = health;
        dot.effectPrefab = effectPrefab;
    }

    public override void ModifyProjectile(GameObject owner, AbilityInstance instance, ProjectileData data)
    {
        if (!MatchesSlot(instance, data.slot)) return;
        data.OnHit += (target, hitPoint, hitDir) => ApplyDoT(target, owner);
    }

    public override void ModifyMelee(GameObject owner, AbilityInstance instance, MeleeData melee)
    {
        if (!MatchesSlot(instance, melee.slot)) return;
        melee.OnHit += (target) => ApplyDoT(target, owner);
    }
}
