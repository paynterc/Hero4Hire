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
    public Action<GameObject> OnHit;
}
