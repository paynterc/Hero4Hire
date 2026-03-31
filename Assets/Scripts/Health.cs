using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public bool isInvulnerable = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable) return;

        currentHealth -= amount;

        Debug.Log("Took damage: " + amount);

        if (currentHealth <= 0)
        {
            Debug.Log("Dead");
        }
    }
}
