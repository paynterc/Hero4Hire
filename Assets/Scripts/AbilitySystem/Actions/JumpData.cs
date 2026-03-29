using UnityEngine;

public class JumpData : IActionData { 

	public ActionSlot slot;
    public AudioClip jumpSound;
    public float jumpForce = 10f;
    public float energyCost = 0f;
}