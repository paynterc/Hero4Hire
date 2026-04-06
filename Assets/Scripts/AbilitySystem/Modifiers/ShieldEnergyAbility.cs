using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Shield Energy")]
public class ShieldEnergyAbility : Ability
{
    public float energyPerHit = 15f;

    public override void OnActivateShield(GameObject owner, AbilityInstance instance, ShieldInstance shield)
    {
        shield.OnHit += (attacker) =>
        {
            owner.GetComponent<Energy>()?.Add(energyPerHit);
        };
    }
}
