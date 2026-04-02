using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public bool isInvulnerable = false;

    public event Action OnDeath;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable || isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);

        Debug.Log("Took damage: " + amount);

        if (currentHealth <= 0)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }
}
