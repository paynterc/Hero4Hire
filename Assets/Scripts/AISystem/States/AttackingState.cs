using System;
using UnityEngine;

[Serializable]
public class AttackingState : AIState
{
    public ActionSlot attackSlot = ActionSlot.Primary;

    public override void OnEnter(AIContext ctx)
    {
        if (ctx.AgentReady)
            ctx.agent.ResetPath();

        ctx.abilitySystem?.SetHeld(attackSlot, true);
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
        ctx.abilitySystem?.SetHeld(attackSlot, false);
    }
}
