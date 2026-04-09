using System;
using UnityEngine;

[Serializable]
public class TargetIsCarriedByOtherCondition : AICondition
{
    public override bool Evaluate(AIContext ctx)
    {
        if (ctx.target == null) return false;
        if (ctx.target.GetComponent<CarriedMarker>() == null) return false;
        return ctx.target.transform.parent != ctx.owner.transform;
    }
}
