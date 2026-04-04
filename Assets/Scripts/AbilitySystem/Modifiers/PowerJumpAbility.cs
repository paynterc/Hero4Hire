using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Power Jump")]
public class PowerJumpAbility : Ability
{
    public float bonusForce = 10;

    public override void ModifyJump(GameObject owner, AbilityInstance instance, JumpData jump)
    {
        if (!MatchesSlot(instance, jump.slot)) return;
        
        jump.jumpForce += bonusForce;
        jump.energyCost += energyCost;
        
    }

}
