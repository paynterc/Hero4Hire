using System;
using UnityEngine;
using UnityEngine.AI;

public enum StrafeDirection { Left, Right, Random }

[Serializable]
public class StrafeState : AIState
{
    public StrafeDirection direction = StrafeDirection.Random;
    public float strafeDistance = 4f;
    public float stoppingDistance = 0.3f;

    public override void OnEnter(AIContext ctx)
    {
        if (ctx.agent == null) return;
        ctx.agent.stoppingDistance = stoppingDistance;
        ctx.agent.updateRotation = false;

        SetStrafeDestination(ctx);
    }

    public override void OnUpdate(AIContext ctx)
    {
        if (ctx.target == null) return;

        Vector3 dir = ctx.target.transform.position - ctx.owner.transform.position;
        dir.y = 0f;
        if (dir != Vector3.zero)
            ctx.owner.transform.rotation = Quaternion.LookRotation(dir);
    }

    public override void OnExit(AIContext ctx)
    {
        if (ctx.agent == null) return;
        ctx.agent.updateRotation = true;
        ctx.agent.ResetPath();
    }

    void SetStrafeDestination(AIContext ctx)
    {
        bool goLeft = direction switch
        {
            StrafeDirection.Left => true,
            StrafeDirection.Right => false,
            _ => UnityEngine.Random.value < 0.5f
        };

        Vector3 sideways = goLeft
            ? -ctx.owner.transform.right
            : ctx.owner.transform.right;

        Vector3 destination = ctx.owner.transform.position + sideways * strafeDistance;

        if (NavMesh.SamplePosition(destination, out NavMeshHit hit, strafeDistance, NavMesh.AllAreas))
            ctx.agent.SetDestination(hit.position);
    }
}
