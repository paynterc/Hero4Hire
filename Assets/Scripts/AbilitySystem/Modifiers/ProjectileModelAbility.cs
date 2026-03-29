using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Projectile Model")]
public class ProjectileModelAbility : Ability
{
    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;

    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
    	if (!MatchesSlot(instance, shot.slot)) return;
        shot.projectilePrefab = projectilePrefab;
        shot.muzzleFlashPrefab = muzzleFlashPrefab;
    }
}
