using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public bool isInvulnerable = false;
    public Health damageRedirect;

    public event Action OnDeath;
    public event Action<GameObject> OnDamage;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount, GameObject attacker = null)
    {
        if (damageRedirect != null)
        {
            damageRedirect.TakeDamage(amount, attacker);
            return;
        }
        if (isInvulnerable || isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);

        Debug.Log("Took damage: " + amount);

        OnDamage?.Invoke(attacker);

        if (currentHealth <= 0)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }
}
