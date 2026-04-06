using System;
using UnityEngine;

public class ShieldInstance : MonoBehaviour
{
    public event Action<GameObject> OnHit;
    public event Action OnExpired;
    public event Action OnBroke;

    public Health shieldHealth { get; private set; }

    private float duration;
    private float elapsed;
    private Health playerHealth;

    public void Initialize(ShieldData data, GameObject owner)
    {
        duration = data.duration;
        playerHealth = owner.GetComponent<Health>();

        shieldHealth = GetComponent<Health>();
        if (shieldHealth == null)
            shieldHealth = gameObject.AddComponent<Health>();

        shieldHealth.maxHealth = data.shieldHealth;
        shieldHealth.OnDeath += HandleBroke;
        shieldHealth.OnDamage += (attacker) => OnHit?.Invoke(attacker);

        if (playerHealth != null)
            playerHealth.damageRedirect = shieldHealth;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
            Expire();
    }

    void Expire()
    {
        Cleanup();
        OnExpired?.Invoke();
        Destroy(gameObject);
    }

    void HandleBroke()
    {
        Cleanup();
        OnBroke?.Invoke();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        Cleanup();
    }

    void Cleanup()
    {
        if (playerHealth != null && playerHealth.damageRedirect == shieldHealth)
            playerHealth.damageRedirect = null;
    }
}
