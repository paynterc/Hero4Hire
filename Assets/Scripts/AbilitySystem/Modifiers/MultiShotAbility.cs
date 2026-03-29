using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Multi Shot")]
public class MultiShotAbility : Ability
{
    public int extraProjectiles = 2;

    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
    	if (!MatchesSlot(instance, shot.slot)) return;
        shot.projectileCount += extraProjectiles;
    }
}
