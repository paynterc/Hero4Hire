using System;
using UnityEngine;

[Serializable]
public class ReturnToSpawnState : AIState
{
    public float stoppingDistance = 0.5f;

    public override void OnEnter(AIContext ctx)
    {
        if (ctx.agent == null) return;
        ctx.agent.stoppingDistance = stoppingDistance;
        ctx.agent.updateRotation = true;
        ctx.agent.SetDestination(ctx.spawnPosition);
    }

    public override void OnExit(AIContext ctx)
    {
        if (ctx.agent != null)
            ctx.agent.ResetPath();
    }
}
