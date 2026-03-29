using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private GameObject impactEffect;

    public float lifetime = 3f;

    public void Initialize(Vector3 dir, float spd, GameObject impactFx)
    {
        direction = dir.normalized;
        speed = spd;
        impactEffect = impactFx;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // Spawn impact effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Optional: damage
        var health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(10);
        }

        Destroy(gameObject);
    }
}
