using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    public List<AIStateNode> nodes;

    private AIStateNode currentNode;
    private AIContext ctx;

    void Start()
    {
        ctx = new AIContext
        {
            owner = gameObject,
            agent = GetComponent<NavMeshAgent>(),
            abilitySystem = GetComponent<AbilitySystem>()
        };

        if (nodes != null && nodes.Count > 0)
        {
            currentNode = nodes[0];
            currentNode.state?.OnEnter(ctx);
        }
    }

    void Update()
    {
        if (currentNode == null) return;

        ctx.stateTime += Time.deltaTime;
        currentNode.state?.OnUpdate(ctx);

        foreach (var transition in currentNode.transitions)
        {
            if (transition.Evaluate(ctx))
            {
                TransitionTo(transition.targetNodeName);
                break;
            }
        }
    }

    void TransitionTo(string nodeName)
    {
        currentNode?.state?.OnExit(ctx);
        ctx.stateTime = 0f;
        ctx.auxTimer = 0f;
        currentNode = nodes.Find(n => n.name == nodeName);
        currentNode?.state?.OnEnter(ctx);
    }
}
