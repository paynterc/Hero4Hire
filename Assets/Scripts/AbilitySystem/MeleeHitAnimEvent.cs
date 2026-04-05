using UnityEngine;

// Attach to the same GameObject as the Animator.
// In the animation clip, add an event that calls OnMeleeHit() at the impact frame.
public class MeleeHitAnimEvent : MonoBehaviour
{
    public ActionSlot slot = ActionSlot.Primary;
    private AbilitySystem abilitySystem;

    void Start() => abilitySystem = GetComponentInParent<AbilitySystem>();

    // Called by Unity animation event
    public void OnMeleeHit() => abilitySystem?.TriggerMeleeHit(slot);
}
