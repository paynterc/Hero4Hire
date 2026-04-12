using UnityEngine;

// Placed on the body GameObject (which owns the Animator).
// Forwards OnAnimatorIK up to PlayerIK on the parent, since Unity only
// fires OnAnimatorIK on the same GameObject as the Animator.
public class AnimatorIKRelay : MonoBehaviour
{
    [HideInInspector] public PlayerIK playerIK;

    void OnAnimatorIK(int layerIndex)
    {
        playerIK?.RelayAnimatorIK(layerIndex);
    }
}
