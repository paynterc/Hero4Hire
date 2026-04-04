using System;

[Serializable]
public class TargetAcquiredCondition : AICondition
{
    public override bool Evaluate(AIContext ctx) => ctx.target != null;
}
