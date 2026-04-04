using System;

[Serializable]
public class WasHitCondition : AICondition
{
    public override bool Evaluate(AIContext ctx)
    {
        if (!ctx.wasHit) return false;

        ctx.target = ctx.lastAttacker;
        ctx.wasHit = false;
        return true;
    }
}
