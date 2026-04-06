using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Shield Spikes")]
public class ShieldSpikesAbility : Ability
{
    public int damagePerHit = 10;

    public override void OnActivateShield(GameObject owner, AbilityInstance instance, ShieldInstance shield)
    {
        shield.OnHit += (attacker) =>
        {
            if (attacker == null) return;
            var health = attacker.GetComponent<Health>() ?? attacker.GetComponentInParent<Health>();
            health?.TakeDamage(damagePerHit, owner);
        };
    }
}
