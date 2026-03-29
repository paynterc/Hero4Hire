using System;

[Serializable]
public class TimeoutCondition : AICondition
{
    public float duration = 5f;

    public override bool Evaluate(AIContext ctx)
    {
        return ctx.stateTime >= duration;
    }
}
