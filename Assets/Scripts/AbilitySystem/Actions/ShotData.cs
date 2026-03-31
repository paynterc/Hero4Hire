using UnityEngine;

public class ShotData : IActionData
{
    public Vector3 origin;
    public Vector3 direction;

    public float force = 1000f;

    public int projectileCount = 1;
    public float spreadAngle = 0f;

    public GameObject projectilePrefab;

    public GameObject muzzleFlashPrefab;
    public AudioClip fireSound;
    public ActionSlot slot;
	public ActionContext context;
	public float energyCost = 0f;
	public int damage = 10;

}
