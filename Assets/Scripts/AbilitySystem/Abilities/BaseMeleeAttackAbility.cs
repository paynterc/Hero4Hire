using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Melee Attack")]
public class BaseMeleeAttackAbility : Ability
{
    public int damage = 20;
    public float range = 2f;
    [Range(0f, 360f)]
    public float attackAngle = 90f;
    public float attackRate = 0.5f;
    public LayerMask targetLayers;
    public AudioClip attackSound;
    public string attackAnimatorTrigger = "Attack";
	public GameObject slashPrefab;
	
	
	public float slashYpos = .05f;
	public float slashXrot = 90f;
	public float slashYrot = 0f;
	public float slashZrot = 0f;
	
    [Tooltip("When enabled, hit detection waits for an animation event rather than firing immediately on key press.")]
    public bool useAnimationEvents = false;

    public override void InitializeMelee(GameObject owner, AbilityInstance instance, MeleeData melee)
    {
        if (!MatchesSlot(instance, melee.slot)) return;
        melee.damage = damage;
        melee.range = range;
        melee.attackAngle = attackAngle;
        melee.energyCost = energyCost;
        melee.targetLayers = targetLayers;
    }

    public override void OnUpdate(GameObject owner, AbilityInstance instance)
    {
        var system = owner.GetComponent<AbilitySystem>();
        if (!system.IsHeld(instance.slot)) return;
        if (!instance.CanFire()) return;

        var energy = owner.GetComponent<Energy>();
        if (energy == null || !energy.HasEnough(energyCost)) return;

        energy.Spend(energyCost);
        instance.TriggerFireRate(attackRate);
        
        Vector3 spawnPos = owner.transform.position + owner.transform.forward + Vector3.up * slashYpos;
        Quaternion spawnRot = owner.transform.rotation * Quaternion.Euler(slashXrot, slashYrot, slashZrot);
		Instantiate(slashPrefab, spawnPos, spawnRot);
		
        if (!string.IsNullOrEmpty(attackAnimatorTrigger))
            owner.GetComponentInChildren<Animator>()?.SetTrigger(attackAnimatorTrigger);

        if (attackSound != null)
            AudioSource.PlayClipAtPoint(attackSound, owner.transform.position);

        if (!useAnimationEvents)
            system.TriggerMeleeHit(instance.slot);
    }

    public override void OnMeleeHit(GameObject owner, AbilityInstance instance, MeleeData melee)
    {
        if (!MatchesSlot(instance, melee.slot)) return;

        var system = owner.GetComponent<AbilitySystem>();
        Transform origin = system.firePoint != null ? system.firePoint : owner.transform;

        LayerMask mask = melee.targetLayers.value == 0 ? Physics.DefaultRaycastLayers : melee.targetLayers;
        var hits = Physics.OverlapSphere(origin.position, melee.range, mask);

        foreach (var hit in hits)
        {
            if (hit.gameObject == owner) continue;

            Vector3 toTarget = (hit.transform.position - origin.position).normalized;
            if (Vector3.Angle(origin.forward, toTarget) > melee.attackAngle * 0.5f) continue;

            var health = hit.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage((int)melee.damage, melee.context?.owner);
                melee.OnHit?.Invoke(hit.gameObject);
            }
        }
    }
}
