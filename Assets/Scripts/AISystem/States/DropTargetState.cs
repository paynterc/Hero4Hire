using System;
using UnityEngine;

[Serializable]
public class DropTargetState : AIState
{
    public Vector3 dropOffset = new Vector3(0f, 0f, 1.5f);

    public override void OnEnter(AIContext ctx)
    {
        if (ctx.carriedObject == null || ctx.carriedObject.transform.parent != ctx.owner.transform) return;

        // Place in front of bot before releasing
        ctx.carriedObject.transform.position = ctx.owner.transform.TransformPoint(dropOffset);

        ctx.owner.GetComponent<AIBrain>()?.DropCarried();
    }
}
