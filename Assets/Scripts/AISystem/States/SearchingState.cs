using System;
using UnityEngine;

[Serializable]
public class SearchingState : AIState
{
    public float detectionRange = 10f;
    public LayerMask targetLayers;

    public override void OnUpdate(AIContext ctx)
    {
        var hits = Physics.OverlapSphere(ctx.owner.transform.position, detectionRange, targetLayers);
        foreach (var hit in hits)
        {
            if (hit.gameObject != ctx.owner)
            {
                ctx.target = hit.gameObject;
                return;
            }
        }
        ctx.target = null;
    }
}
