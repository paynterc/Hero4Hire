using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    public List<AIStateNode> nodes;

    public string currentStateName;

    private AIStateNode currentNode;
    private AIContext ctx;
    private Animator animator;

    void Start()
    {
        ctx = new AIContext
        {
            owner = gameObject,
            agent = GetComponent<NavMeshAgent>(),
            abilitySystem = GetComponent<AbilitySystem>(),
            spawnPosition = transform.position,
            animator = GetComponent<Animator>()
        };

        var health = GetComponent<Health>();
        if (health != null)
            health.OnDamage += (attacker) => { ctx.wasHit = true; ctx.lastAttacker = attacker; };

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
		currentStateName = currentNode.name;
        foreach (var transition in currentNode.transitions)
        {
            if (transition.Evaluate(ctx))
            {
                TransitionTo(transition.targetNodeName);
                break;
            }
        }
		float speed = ctx.AgentReady ? ctx.agent.velocity.magnitude : 0f;
    	if(ctx.animator)
    	{
			ctx.animator.SetFloat("Speed", speed);        
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

    public void ClearTargetIfMatches(GameObject obj)
    {
        if (ctx != null && ctx.target == obj)
            ctx.target = null;
    }

    public void DropCarried()
    {
        if (ctx == null || ctx.carriedObject == null) return;

        var carried = ctx.carriedObject;

        carried.transform.SetParent(null, true);

        var marker = carried.GetComponent<CarriedMarker>();
        if (marker != null)
            Destroy(marker);

        var rb = carried.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;

        var navAgent = carried.GetComponent<NavMeshAgent>();
        if (navAgent != null) navAgent.enabled = true;

        var ai = carried.GetComponent<AIBrain>();
        if (ai != null) ai.enabled = true;

        var player = carried.GetComponent<PlayerController>();
        if (player != null) player.enabled = true;

        var abilities = carried.GetComponent<AbilitySystem>();
        if (abilities != null) abilities.enabled = true;

        ctx.carriedObject = null;
        
        var ikHandler = GetComponent<IKHandler>();
        if(ikHandler != null){
        	ikHandler.ikOn = false;
        }
        
    }
}
