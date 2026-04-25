using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool isInvulnerable = false;
    public Health damageRedirect;

    public event Action OnDeath;
    public event Action<GameObject> OnDamage;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, GameObject attacker = null)
    {
        if (damageRedirect != null)
        {
            damageRedirect.TakeDamage(amount, attacker);
            return;
        }
        if (isInvulnerable || isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0f);

        Debug.Log($"{gameObject.name} Took damage: " + amount);

        OnDamage?.Invoke(attacker);

        if (currentHealth <= 0f)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }
}
