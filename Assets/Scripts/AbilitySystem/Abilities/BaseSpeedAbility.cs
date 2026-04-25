using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Speed Ability")]
public class BaseSpeedAbility : Ability
{
	public float pct = 10f;

    public override void OnEquip(GameObject owner, AbilityInstance instance)
    {
		var agent = owner.GetComponent<PlayerController>();
		if(agent)
		{
			var mod = 1f + pct / 100f;
			agent.moveSpeed *= mod;

		}

    }


}
