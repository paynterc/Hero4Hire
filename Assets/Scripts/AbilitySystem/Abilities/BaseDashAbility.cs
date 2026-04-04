using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Dash Ability")]
public class BaseDashAbility : Ability
{

    public AudioClip sound;    
    public float force = 20f;
    public float duration = 0.2f;
    public float cooldown = 1f;
    

    public override void InitializeDash(GameObject owner, AbilityInstance instance, DashData dash)
    {
        if (!MatchesSlot(instance, dash.slot)) return;
        Debug.Log($"Initialize Base Dash force: {dash.force}");
        dash.force = force;
        dash.duration = duration;
        dash.cooldown = cooldown;
        dash.sound = sound;
    }

	public override void OnDash(GameObject owner, AbilityInstance instance, DashData dash)
    {
        
        var energy = owner.GetComponent<Energy>();
    	if (energy == null) return;
    	
        if (!instance.IsReady()) return;
		if (!energy.HasEnough(dash.energyCost)) return;
		

        var controller = owner.GetComponent<PlayerController>();

        Vector3 dashDir = controller.lastMoveDirection;

        // Prevent dash if still no valid direction
        if (dashDir.magnitude < 0.1f)
            return;
        
        energy.Spend(dash.energyCost);

        controller.overrideMovement = true;
        controller.externalVelocity = dashDir * dash.force;

        instance.TriggerCooldown(dash.cooldown);

        owner.GetComponent<MonoBehaviour>()
            .StartCoroutine(DashRoutine(controller, dash));
    }

    private System.Collections.IEnumerator DashRoutine(PlayerController controller, DashData dash)
    {
        //yield return new WaitForSeconds(dash.duration);
        //controller.overrideMovement = false;
        
        var health = controller.GetComponent<Health>();

		if (health != null)
			health.isInvulnerable = true;

		yield return new WaitForSeconds(duration);

		if (health != null)
			health.isInvulnerable = false;

		controller.overrideMovement = false;
        
    }
    

}
