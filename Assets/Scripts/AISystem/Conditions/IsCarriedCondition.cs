using System;

[Serializable]
public class IsCarriedCondition : AICondition
{
    public override bool Evaluate(AIContext ctx)
    {
        if (ctx.target == null) return false;
        return ctx.target.GetComponent<CarriedMarker>() != null;
    }
}
