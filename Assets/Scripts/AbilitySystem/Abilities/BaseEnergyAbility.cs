using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Energy Ability")]
public class BaseEnergyAbility : Ability
{
	public float energyPct = 10f;

    public override void OnEquip(GameObject owner, AbilityInstance instance)
    {
        var energy = owner.GetComponent<Energy>();
		if(energy)
		{
			
			energy.maxEnergy *=  ((100f+energyPct)/100);
		
			energy.currentEnergy = energy.maxEnergy;

		}

    }


}
