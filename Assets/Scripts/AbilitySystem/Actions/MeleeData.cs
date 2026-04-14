using UnityEngine;
using System;

public class MeleeData : IActionData
{
    public ActionSlot slot;
    public float damage = 20f;
    public float range = 2f;
    public float attackAngle = 90f;
    public float energyCost = 0f;
    public LayerMask targetLayers;
    public ActionContext context;
    public GameObject impactPrefab;
    public Action<GameObject> OnHit;
    public float attackRate = .5f;
    public float windupTime = 0f;
    public string windupAnimatorTrigger = "";
    public GameObject windupPrefab;
}
