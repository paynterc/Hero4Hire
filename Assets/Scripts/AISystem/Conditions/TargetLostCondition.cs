using System;

[Serializable]
public class TargetLostCondition : AICondition
{
    public override bool Evaluate(AIContext ctx) => ctx.target == null;
}
