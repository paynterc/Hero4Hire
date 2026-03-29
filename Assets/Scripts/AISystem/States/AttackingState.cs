using System;
using UnityEngine;

[Serializable]
public class AttackingState : AIState
{
    public ActionSlot attackSlot = ActionSlot.Primary;
    public float attackInterval = 1f;

    public override void OnEnter(AIContext ctx)
    {
        if (ctx.agent != null)
            ctx.agent.ResetPath();

        ctx.auxTimer = attackInterval;
    }

    public override void OnUpdate(AIContext ctx)
    {
        if (ctx.target == null) return;

        Vector3 dir = ctx.target.transform.position - ctx.owner.transform.position;
        dir.y = 0f;
        if (dir != Vector3.zero)
            ctx.owner.transform.rotation = Quaternion.LookRotation(dir);

        ctx.auxTimer += Time.deltaTime;
        if (ctx.auxTimer >= attackInterval)
        {
            ctx.auxTimer = 0f;
            ctx.abilitySystem?.Fire(attackSlot);
        }
    }
}
