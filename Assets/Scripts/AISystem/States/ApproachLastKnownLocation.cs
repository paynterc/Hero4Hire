using System;
using UnityEngine;

[Serializable]
public class ApproachLastKnownLocation : AIState
{
    public float stoppingDistance = 1f;

    public override void OnEnter(AIContext ctx)
    {
        if (!ctx.AgentReady) return;
        ctx.agent.stoppingDistance = stoppingDistance;
        ctx.agent.SetDestination(ctx.lastKnownTargetPosition);
    }

    public override void OnExit(AIContext ctx)
    {
        if (ctx.AgentReady)
            ctx.agent.ResetPath();
    }
}
