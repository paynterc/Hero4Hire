using UnityEngine;

public class AbilityProjectile : MonoBehaviour
{
    private ProjectileData data;
    private float lifeTimer;

    public void Init(ProjectileData projectileData)
    {
        data = projectileData;
        lifeTimer = data.lifetime;

        data.OnSpawn?.Invoke(gameObject);
    }

    void Update()
    {
        // Lifetime handling
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        data.OnHit?.Invoke(collision.gameObject);

        Destroy(gameObject);
    }
}
