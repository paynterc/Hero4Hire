using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Base Teleport Ability")]
public class BaseTeleportAbility : Ability
{
    public float range = 10f;
    public float cooldown = 3f;
    public AudioClip sound;
    public GameObject teleportFxPrefab;

    [Tooltip("Layers that block the teleport path.")]
    public LayerMask blockingLayers;

    [Tooltip("Capsule radius used for the sweep — should match the player's capsule collider.")]
    public float capsuleRadius = 0.4f;

    public override void InitializeDash(GameObject owner, AbilityInstance instance, DashData dash)
    {
        if (!MatchesSlot(instance, dash.slot)) return;
        dash.energyCost = energyCost;
        dash.cooldown   = cooldown;
    }

    public override void OnDash(GameObject owner, AbilityInstance instance, DashData dash)
    {
        if (!instance.IsReady()) return;

        var energy = owner.GetComponent<Energy>();
        if (energy != null && !energy.HasEnough(dash.energyCost)) return;

        var controller = owner.GetComponent<PlayerController>();
        if (controller == null) return;

        Vector3 dir = controller.lastMoveDirection;
        if (dir.magnitude < 0.1f) return;
        dir.Normalize();

        // Use a capsule cast so the player won't teleport inside geometry
        Vector3 origin = owner.transform.position;
        float   height = GetCapsuleHeight(owner);
        Vector3 bottom = origin + Vector3.up * capsuleRadius;
        Vector3 top    = origin + Vector3.up * (height - capsuleRadius);

        Vector3 destination;
        if (Physics.CapsuleCast(bottom, top, capsuleRadius, dir, out RaycastHit hit, range, blockingLayers))
        {
            // Stop just before the hit surface
            float safeDistance = Mathf.Max(0f, hit.distance - capsuleRadius);
            destination = origin + dir * safeDistance;
        }
        else
        {
            destination = origin + dir * range;
        }

        energy?.Spend(dash.energyCost);
        instance.TriggerCooldown(cooldown);

        if (sound != null)
            AudioSource.PlayClipAtPoint(sound, origin);

        if (teleportFxPrefab != null)
        {
            Object.Instantiate(teleportFxPrefab, origin, owner.transform.rotation);
            Object.Instantiate(teleportFxPrefab, destination, owner.transform.rotation);
        }

        // Disable the CharacterController briefly so we can reposition
        var cc = owner.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        owner.transform.position = destination;
        if (cc != null) cc.enabled = true;
    }

    float GetCapsuleHeight(GameObject owner)
    {
        var cc = owner.GetComponent<CharacterController>();
        return cc != null ? cc.height : 2f;
    }
}
