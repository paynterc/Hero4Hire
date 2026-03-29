using System;
using UnityEngine;

[Serializable]
public class ApproachingState : AIState
{
    public float stoppingDistance = 2f;

    public override void OnEnter(AIContext ctx)
    {
        if (ctx.agent != null)
            ctx.agent.stoppingDistance = stoppingDistance;
    }

    public override void OnUpdate(AIContext ctx)
    {
        if (ctx.target != null)
            ctx.lastKnownTargetPosition = ctx.target.transform.position;

        if (ctx.agent != null)
            ctx.agent.SetDestination(ctx.lastKnownTargetPosition);
    }

    public override void OnExit(AIContext ctx)
    {
        if (ctx.agent != null)
            ctx.agent.ResetPath();
    }
}
