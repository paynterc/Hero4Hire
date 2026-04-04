using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Rapid Shot")]
public class RapidShotAbility : Ability
{
    public float reduceFireRate = 0.05f;

    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
    	if (!MatchesSlot(instance, shot.slot)) return;
        shot.fireRate -= reduceFireRate;
    }
}
