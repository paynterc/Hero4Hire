using System;
using UnityEngine;

[Serializable]
public class TargetInRangeCondition : AICondition
{
    public float range = 5f;

    public override bool Evaluate(AIContext ctx)
    {
        if (!ctx.AgentReady) return false;
        if (ctx.target == null) return false;
        return Vector3.Distance(ctx.owner.transform.position, ctx.target.transform.position) <= range;
    }
}
