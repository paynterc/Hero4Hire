using UnityEngine;
using UnityEngine.AI;

public class KnockbackReceiver : MonoBehaviour
{
    private Vector3 velocity;
    private float duration;
    private float elapsed;
    private float damping;

    private Rigidbody rb;
    private CharacterController cc;
    private PlayerController pc;
    private NavMeshAgent agent;

    public static void Apply(GameObject target, Vector3 direction, float force, float duration, float damping)
    {
        var rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
            return;
        }

        // Reuse existing receiver rather than stacking
        var receiver = target.GetComponent<KnockbackReceiver>();
        if (receiver == null)
            receiver = target.AddComponent<KnockbackReceiver>();

        receiver.velocity = direction * force;
        receiver.duration = duration;
        receiver.damping = damping;
        receiver.elapsed = 0f;
        receiver.Init();
    }

    void Init()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();

        if (agent != null) agent.enabled = false;
        if (pc != null) pc.overrideMovement = true;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        velocity = Vector3.Lerp(velocity, Vector3.zero, damping * Time.deltaTime);

        if (cc != null)
        {
            if (pc != null)
                pc.externalVelocity = velocity;
            else
                cc.Move(velocity * Time.deltaTime);
        }

        if (elapsed >= duration)
        {
            if (agent != null) agent.enabled = true;
            if (pc != null) pc.overrideMovement = false;
            Destroy(this);
        }
    }
}
