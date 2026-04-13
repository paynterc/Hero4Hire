using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Jump Ability")]
public class BaseJumpAbility : Ability
{

    public AudioClip jumpSound;
    public float jumpForce = 1000f;
    
    public override void InitializeJump(GameObject owner, AbilityInstance instance, JumpData jump)
    {
        if (!MatchesSlot(instance, jump.slot)) return;
        jump.jumpForce = jumpForce;
        jump.jumpSound = jumpSound;
    }


    public override void OnJump(GameObject owner, AbilityInstance instance, JumpData jump)
    {
    	
    	var energy = owner.GetComponent<Energy>();
    	if (energy == null) return;
    	if (!energy.HasEnough(jump.energyCost)) return;

    	var player = owner.GetComponent<PlayerController>();


        if (player.isGrounded)
        {
        	energy.Spend(jump.energyCost);
            player.yVelocity = jump.jumpForce;
        }
    }

}

