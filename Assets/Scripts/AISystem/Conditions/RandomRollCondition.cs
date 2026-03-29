using System;
using UnityEngine;

[Serializable]
public class RandomRollCondition : AICondition
{
    [Range(0f, 1f)]
    public float probability = 0.01f;

    public override bool Evaluate(AIContext ctx)
    {
        return UnityEngine.Random.value < probability;
    }
}
