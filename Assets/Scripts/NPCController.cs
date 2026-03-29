using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public Transform target;

    public float attackRange = 10f;
    public float stopDistance = 5f;

    private AbilitySystem abilitySystem;
    private NavMeshAgent agent;

    void Awake()
    {
        abilitySystem = GetComponent<AbilitySystem>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // --- Movement ---
        if (distance > stopDistance)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.ResetPath(); // stop moving
        }

        // --- Face target (optional override) ---
        Vector3 direction = (target.position - transform.position);
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // --- Line of sight ---
        LayerMask mask = LayerMask.GetMask("Obstacle", "Player");
        bool hasLineOfSight = HasLineOfSight(target, attackRange, mask);
        bool inRange = distance <= attackRange;

        // --- Attack logic ---
        bool shouldFire = inRange && hasLineOfSight;
        //Debug.Log($"inRange: {inRange}");
        //Debug.Log($"hasLineOfSight: {hasLineOfSight}");


        abilitySystem.SetHeld(ActionSlot.Primary, shouldFire);
    }

    public bool HasLineOfSight(Transform target, float range, LayerMask mask)
    {
        if (target == null) return false;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 targetPos = target.position + Vector3.up * 0.5f;

        Vector3 direction = (targetPos - origin).normalized;
        float distance = Vector3.Distance(origin, targetPos);

        if (distance > range)
            distance = range;

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, distance, mask, QueryTriggerInteraction.Ignore))
        {
        	//Debug.DrawLine(transform.position, hit.point, Color.red);
        	//Debug.Log("Hit: " + hit.collider.gameObject.name);
            return hit.transform == target;
        }else{
        	//Debug.DrawLine(transform.position, transform.position + transform.forward * 10f, Color.green);
        }

        return false;
    }
}
