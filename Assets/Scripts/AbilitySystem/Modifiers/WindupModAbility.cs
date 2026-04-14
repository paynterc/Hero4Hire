using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Wind-up")]
public class WindupModAbility : Ability
{
	[Header("Adds a wind-up time, animation and prefab to attack.")]
    public float windupInSeconds = 1f;
    public string windupAnimatorTrigger = "";
	public GameObject windupPrefab;

    public override void ModifyShot(GameObject owner, AbilityInstance instance, ShotData shot)
    {
    	if (!MatchesSlot(instance, shot.slot)) return;
    	
        shot.windupTime += windupInSeconds;
        if(windupAnimatorTrigger!="")
        	shot.windupAnimatorTrigger = windupAnimatorTrigger;
        if(windupPrefab != null)
        	shot.windupPrefab = windupPrefab;        	
        
    }
    
    public override void ModifyMelee(GameObject owner, AbilityInstance instance, MeleeData melee)
    {
        if (!MatchesSlot(instance, melee.slot)) return;
        melee.windupTime += windupInSeconds;
        if(windupAnimatorTrigger!="")
        	melee.windupAnimatorTrigger = windupAnimatorTrigger;
        if(windupPrefab != null)
        	melee.windupPrefab = windupPrefab;  
    }
}