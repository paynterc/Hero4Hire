using System;
using UnityEngine;

[Serializable]
public class MoveToDestinationState : AIState
{
    public Transform destination;
    public float stoppingDistance = 0.5f;

    public override void OnEnter(AIContext ctx)
    {
        if (!ctx.AgentReady || destination == null) return;
        ctx.agent.stoppingDistance = stoppingDistance;
        ctx.agent.updateRotation = true;
        ctx.agent.SetDestination(destination.position);
    }

    public override void OnExit(AIContext ctx)
    {
        if (ctx.AgentReady)
            ctx.agent.ResetPath();
    }
}
