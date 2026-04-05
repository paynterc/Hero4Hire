using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Knockback")]
public class KnockbackAbility : Ability
{
    public float force = 10f;
    public float duration = 0.3f;
    [Tooltip("How quickly the knockback velocity decays. Higher = stops faster.")]
    public float damping = 8f;

    void ApplyKnockback(GameObject target, Vector3 direction)
    {
        var health = target.GetComponent<Health>() ?? target.GetComponentInParent<Health>();
        if (health == null) return;

        KnockbackReceiver.Apply(health.gameObject, direction.normalized, force, duration, damping);
    }

    public override void ModifyProjectile(GameObject owner, AbilityInstance instance, ProjectileData data)
    {
        if (!MatchesSlot(instance, data.slot)) return;
        data.OnHit += (target, hitPoint, hitDir) => ApplyKnockback(target, hitDir);
    }

    public override void ModifyMelee(GameObject owner, AbilityInstance instance, MeleeData melee)
    {
        if (!MatchesSlot(instance, melee.slot)) return;
        melee.OnHit += (target) =>
        {
            Vector3 dir = (target.transform.position - owner.transform.position).normalized;
            ApplyKnockback(target, dir);
        };
    }
}
