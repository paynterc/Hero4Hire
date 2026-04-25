using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Regen Ability")]
public class BaseRegenAbility : Ability
{
    public float regenPerSecond = 5f;



    public override void OnUpdate(GameObject owner, AbilityInstance instance)
    {
        var health = owner.GetComponent<Health>();
    	if(health != null && health.currentHealth<health.maxHealth)
    	{
			health.currentHealth += regenPerSecond * Time.deltaTime;
			health.currentHealth = Mathf.Min(health.currentHealth, health.maxHealth);
        }
    }


}
