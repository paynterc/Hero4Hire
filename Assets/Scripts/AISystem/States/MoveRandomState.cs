using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class MoveRandomState : AIState
{
    public float wanderRadius = 10f;
    public float stoppingDistance = 0.5f;

    public override void OnEnter(AIContext ctx)
    {
        if (!ctx.AgentReady) return;
        ctx.agent.stoppingDistance = stoppingDistance;
        ctx.agent.updateRotation = true;

        Vector3 randomPoint = ctx.owner.transform.position + UnityEngine.Random.insideUnitSphere * wanderRadius;
        randomPoint.y = ctx.owner.transform.position.y;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            ctx.agent.SetDestination(hit.position);
    }

    public override void OnExit(AIContext ctx)
    {
        if (ctx.AgentReady)
            ctx.agent.ResetPath();
    }
}
