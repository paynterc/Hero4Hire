using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Power Shot")]
public class PowerShotAbility : Ability
{
    public float bonusForce = 500f;

    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
    	if (!MatchesSlot(instance, shot.slot)) return;
        shot.force += bonusForce;
    }
}

