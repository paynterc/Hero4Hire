using UnityEngine;

public class Energy : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float currentEnergy;

    public float regenRate = 10f; // per second

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        Regenerate();
    }

    void Regenerate()
    {
        currentEnergy += regenRate * Time.deltaTime;
        currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
    }

    public bool HasEnough(float amount)
    {
        return currentEnergy >= amount;
    }

    public void Spend(float amount)
    {
        currentEnergy -= amount;
        currentEnergy = Mathf.Max(0f, currentEnergy);
    }

    public void Add(float amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
    }
}
