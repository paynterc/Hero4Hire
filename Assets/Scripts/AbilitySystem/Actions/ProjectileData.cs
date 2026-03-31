using System;
using UnityEngine;

public class ProjectileData
{
	public ActionSlot slot;
    public float speed = 30f;
    public float lifetime = 3f;
    public int damage = 10;

    public ActionContext context;


    
    public Action<GameObject> OnHit;
    public Action<GameObject> OnSpawn;
    public float energyCost = 0f;
}
