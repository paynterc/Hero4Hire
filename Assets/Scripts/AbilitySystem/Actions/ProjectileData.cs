using System;
using UnityEngine;

public class ProjectileData
{
	public ActionSlot slot;
    public float speed = 30f;
    public float lifetime = 3f;
    public float damage = 10f;

    public ActionContext context;

    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;
    
    public Action<GameObject> OnHit;
    public Action<GameObject> OnSpawn;
    public float energyCost = 0f;
}
