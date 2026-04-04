using System;
using UnityEngine;

[Serializable]
public class SearchingState : AIState
{
    public float detectionRange = 10f;
    public LayerMask targetLayers;
    public string targetTag;

    public override void OnUpdate(AIContext ctx)
    {
        LayerMask mask = targetLayers.value == 0 ? Physics.DefaultRaycastLayers : targetLayers;
        var hits = Physics.OverlapSphere(ctx.owner.transform.position, detectionRange, mask);

        GameObject closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.gameObject == ctx.owner) continue;
            if (!string.IsNullOrEmpty(targetTag) && !hit.CompareTag(targetTag)) continue;

            float dist = (hit.transform.position - ctx.owner.transform.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.gameObject;
            }
        }

        ctx.target = closest;
    }
}
