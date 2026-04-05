using UnityEngine;

public class DamageOverTime : MonoBehaviour
{
    [HideInInspector] public int damagePerTick;
    [HideInInspector] public float tickInterval;
    [HideInInspector] public float duration;
    [HideInInspector] public GameObject attacker;
    [HideInInspector] public Health health;
    [HideInInspector] public GameObject effectPrefab;

    private float elapsed;
    private float nextTick;
    private GameObject activeEffect;

    void OnEnable()
    {
        elapsed = 0f;
        nextTick = tickInterval;

        
    }
    
    void Start()
    {
    	if (effectPrefab != null)
    	{
            activeEffect = Instantiate(effectPrefab, transform.position, Quaternion.identity, transform);
    	}
    
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        nextTick -= Time.deltaTime;

        if (nextTick <= 0f)
        {
            nextTick = tickInterval;
            health?.TakeDamage(damagePerTick, attacker);
        }

        if (elapsed >= duration)
            Destroy(this);
    }

    void OnDestroy()
    {
        if (activeEffect != null)
            Destroy(activeEffect);
    }

    public void Refresh(float newDuration)
    {
        elapsed = 0f;
        duration = newDuration;
    }
}
