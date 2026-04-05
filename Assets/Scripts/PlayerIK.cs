using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public float blendSpeed = 8f;

    private Animator animator;
    private AbilitySystem abilitySystem;
    private float ikWeight = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        abilitySystem = GetComponentInParent<AbilitySystem>();
    }

    void Update()
    {
        bool isFiring = abilitySystem != null && abilitySystem.IsIKActive();

        float targetWeight = isFiring ? 1f : 0f;
        ikWeight = Mathf.Lerp(ikWeight, targetWeight, blendSpeed * Time.deltaTime);
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (rightHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
        }

        if (leftHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
    }
}
