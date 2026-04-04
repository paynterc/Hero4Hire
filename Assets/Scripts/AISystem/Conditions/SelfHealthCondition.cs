using System;
using UnityEngine;

[Serializable]
public class SelfHealthCondition : AICondition
{
    public Health health;
    public float lessThan;

    public override bool Evaluate(AIContext ctx)
    {
        return health.currentHealth < lessThan;
    }
}