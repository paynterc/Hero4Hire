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
    public Vector3 spawnPosition;

    public bool wasHit;
    public GameObject lastAttacker;

    public GameObject carriedObject;
    public Animator animator;
}
