using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Rapid Shot")]
public class RapidShotAbility : Ability
{
	[Header("Percentage by which to reduce fire rate. 100=100%")]
    public float reduceFireRatePct = 5f;

    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
    	if (!MatchesSlot(instance, shot.slot)) return;
        shot.fireRate *= (1f - reduceFireRatePct/100f);
    }
}
