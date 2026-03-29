using System;
using UnityEngine;

[Serializable]
public class TargetDetectedCondition : AICondition
{
    public float range = 10f;
    public LayerMask obstacleLayers;

    public override bool Evaluate(AIContext ctx)
    {
        if (ctx.target == null) return false;

        Vector3 ownerPos = ctx.owner.transform.position + Vector3.up;
        Vector3 targetPos = ctx.target.transform.position + Vector3.up;

        if (Vector3.Distance(ownerPos, targetPos) > range) return false;

        Vector3 dir = targetPos - ownerPos;
        return !Physics.Raycast(ownerPos, dir.normalized, dir.magnitude, obstacleLayers);
    }
}
