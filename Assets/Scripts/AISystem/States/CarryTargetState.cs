using System;
using UnityEngine;

[Serializable]
public class CarryTargetState : AIState
{
    public Vector3 attachOffset = new Vector3(0f, 0f, 1.2f);

    private Transform originalParent;
    private bool rbWasKinematic;
    private AIContext thisCtx;

    public override void OnEnter(AIContext ctx)
    {
        if (ctx.target == null) return;
        thisCtx = ctx;

        ctx.carriedObject = ctx.target;

        // Disable control components on the carried object
        SetComponentsEnabled(ctx.carriedObject, false);

        // Freeze physics
        var rb = ctx.carriedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rbWasKinematic = rb.isKinematic;
            rb.isKinematic = true;
        }

        // Attach to bot
        originalParent = ctx.carriedObject.transform.parent;
        ctx.carriedObject.transform.SetParent(ctx.owner.transform, true);
        ctx.carriedObject.transform.localPosition = attachOffset;
        ctx.carriedObject.transform.localRotation = Quaternion.identity;

        // Mark as carried and notify all other brains so they clear this target
        ctx.carriedObject.AddComponent<CarriedMarker>();
        foreach (var brain in UnityEngine.Object.FindObjectsByType<AIBrain>(UnityEngine.FindObjectsSortMode.None))
            brain.ClearTargetIfMatches(ctx.carriedObject);
    }

    public override void OnExit(AIContext ctx)
    {
        // If another bot has stolen the carried object, just clear our reference.
        // DropCarried/DropTargetState handles the full drop in the normal case.
        if (ctx.carriedObject != null && ctx.carriedObject.transform.parent != ctx.owner.transform)
        {
            ctx.carriedObject = null;
        }
    }

    void SetComponentsEnabled(GameObject go, bool enabled)
    {
        
        var navAgent = go.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (navAgent != null)
        {
            if (!enabled) navAgent.ResetPath();
            navAgent.enabled = enabled;
        }
        
        var ai = go.GetComponent<AIBrain>();
        if (ai != null) ai.enabled = enabled;

        var player = go.GetComponent<PlayerController>();
        if (player != null) player.enabled = enabled;

        var abilities = go.GetComponent<AbilitySystem>();
        if (abilities != null)
        {
        	abilities.enabled = enabled;
        } 
        
        var ikHandler = thisCtx.owner.GetComponent<IKHandler>();
        if(ikHandler != null){
        	ikHandler.ikOn = !enabled;
        }else{
        	Debug.Log("No IK Handler");
        }
        

    }
}
