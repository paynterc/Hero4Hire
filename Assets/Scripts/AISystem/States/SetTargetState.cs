using System;
using UnityEngine;

[Serializable]
public class SetTargetState : AIState
{
    public GameObject target;

    public override void OnEnter(AIContext ctx)
    {
        ctx.target = target;
    }
}
