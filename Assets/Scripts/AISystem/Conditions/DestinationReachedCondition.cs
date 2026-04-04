using System;

[Serializable]
public class DestinationReachedCondition : AICondition
{
    public override bool Evaluate(AIContext ctx)
    {
        if (ctx.agent == null) return false;
        if (ctx.agent.pathPending) return false;
        return ctx.agent.remainingDistance <= ctx.agent.stoppingDistance;
    }
}
