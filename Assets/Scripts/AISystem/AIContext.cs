using UnityEngine;
using UnityEngine.AI;

public class AIContext
{
    public GameObject owner;
    public NavMeshAgent agent;
    public AbilitySystem abilitySystem;

    public GameObject target;
    public Vector3 lastKnownTargetPosition;

    public float stateTime;
    public float auxTimer;
}
