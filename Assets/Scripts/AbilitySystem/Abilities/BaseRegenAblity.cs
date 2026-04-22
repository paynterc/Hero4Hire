using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Regen Ability")]
public class BaseRegenAbility : Ability
{
    public float regenPerSecond = 5f;

    private Dictionary<AbilityInstance, float> timers = new Dictionary<AbilityInstance, float>();

    public override void OnEquip(GameObject owner, AbilityInstance instance)
    {
        timers[instance] = 0f;
    }

    public override void OnUnequip(GameObject owner, AbilityInstance instance)
    {
        timers.Remove(instance);
    }

    public override void OnUpdate(GameObject owner, AbilityInstance instance)
    {
        if (!timers.ContainsKey(instance)) return;

        timers[instance] += Time.deltaTime;
        if (timers[instance] < 1f) return;

        timers[instance] -= 1f;

        var health = owner.GetComponent<Health>();
        if (health != null && health.currentHealth < health.maxHealth)
        {
            health.currentHealth = Mathf.Min(health.currentHealth + (int)regenPerSecond, health.maxHealth);
        }
    }
}
