using System;
using UnityEngine;

[Serializable]
public class IsStillCarryingCondition : AICondition
{
    public override bool Evaluate(AIContext ctx)
    {
        if (ctx.carriedObject == null) return false;
        return ctx.carriedObject.transform.parent == ctx.owner.transform;
    }
}
