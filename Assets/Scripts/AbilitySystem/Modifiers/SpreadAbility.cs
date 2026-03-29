using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Spread")]
public class SpreadAbility : Ability
{
    public float spread = 10f;

    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
        if (!MatchesSlot(instance, shot.slot)) return;
        shot.spreadAngle += spread;
    }
}

