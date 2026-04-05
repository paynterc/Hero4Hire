using UnityEngine;

public class AbilityProjectile : MonoBehaviour
{
    private ProjectileData data;
    private float lifeTimer;

    public void Init(ProjectileData projectileData)
    {
        data = projectileData;
        lifeTimer = data.lifetime;

        var ec = GetComponent<ECExplodingProjectile>();
        if (ec != null)
            ec.OnHitCallback += OnHit;

        data.OnSpawn?.Invoke(gameObject);
    }

    void OnHit(GameObject target)
    {
        data.OnHit?.Invoke(target, transform.position, transform.forward);
    }

    void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position;
        data.OnHit?.Invoke(collision.gameObject, hitPoint, transform.forward);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        //data.OnHit?.Invoke(other.gameObject);
        //Destroy(gameObject);
    }
}
