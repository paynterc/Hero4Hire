using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Health Ability")]
public class BaseHealthAbility : Ability
{
	public float bonusHelth = 10f;

    public override void OnEquip(GameObject owner, AbilityInstance instance)
    {
		var health = owner.GetComponent<Health>();
		if(health)
		{
			
			health.maxHealth += bonusHelth;
		
			health.currentHealth = health.maxHealth;

		}

    }


}
