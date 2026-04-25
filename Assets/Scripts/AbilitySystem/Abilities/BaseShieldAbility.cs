using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Shield")]
public class BaseShieldAbility : Ability
{
    public GameObject shieldPrefab;
    public float duration = 5f;
    public float shieldHealth = 100f;
    public float radius = 2f;
    public float cooldown = 15f;
    public float yOffset = 0f;

    public override void InitializeShield(GameObject owner, AbilityInstance instance, ShieldData data)
    {
        if (!MatchesSlot(instance, data.slot)) return;
        data.shieldPrefab = shieldPrefab;
        data.duration = duration;
        data.shieldHealth = shieldHealth;
        data.radius = radius;
        data.energyCost = energyCost;
        data.yOffset = yOffset;
    }

    public override void OnUpdate(GameObject owner, AbilityInstance instance)
    {
        var system = owner.GetComponent<AbilitySystem>();
        if (!system.IsHeld(instance.slot)) return;
        if (!instance.CanFire()) return;

        var energy = owner.GetComponent<Energy>();
        if (energy != null && !energy.HasEnough(energyCost)) return;

        energy?.Spend(energyCost);
        instance.TriggerCooldown(cooldown);

        system.TriggerShield(instance.slot);
    }
}
