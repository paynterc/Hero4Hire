using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Power Dash")]
public class PowerDashAbility : Ability
{
    public float bonusForce = 10;

    public override void ModifyDash(GameObject owner, AbilityInstance instance, DashData dash)
    {
    	if (!MatchesSlot(instance, dash.slot)) return;
    	
        dash.force += bonusForce;
        dash.energyCost += energyCost;


    }
}

