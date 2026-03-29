using System;

[Serializable]
public abstract class AICondition
{
    public abstract bool Evaluate(AIContext ctx);
}
