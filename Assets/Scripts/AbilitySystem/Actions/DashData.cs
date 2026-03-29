using UnityEngine;

public class DashData : IActionData { 

	public ActionSlot slot;
    public AudioClip sound;
    
    public float force = 20f;
    public float duration = 0.2f;
    public float cooldown = 1f;
    public float energyCost = 0f;
}

