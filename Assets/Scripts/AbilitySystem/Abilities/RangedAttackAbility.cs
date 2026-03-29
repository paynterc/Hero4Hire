using UnityEngine;

public class RangedAttackAbility : Ability
{
    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;
    public AudioClip fireSound;

    public float projectileForce = 1000f;

    public float fireRate = 0.2f; // time between shots
    public int projectilesPerShot = 1;
    public float spreadAngle = 5f;

    public override void OnUpdate(GameObject owner, AbilityInstance instance)
    {
        // 🔥 HOLD instead of click
        if (!Input.GetKey(KeyCode.F)) return;

        if (!instance.CanFire()) return;

        var energy = owner.GetComponent<Energy>();
        if (energy == null || !energy.HasEnough(energyCost)) return;

        Fire(owner);

        energy.Spend(energyCost);
        instance.TriggerFireRate(fireRate);
    }

    void Fire(GameObject owner)
    {
        Transform firePoint = owner.transform;

        // 🔥 muzzle flash
        if (muzzleFlashPrefab != null)
        {
            Instantiate(muzzleFlashPrefab,
                firePoint.position + firePoint.forward,
                firePoint.rotation);
        }

        for (int i = 0; i < projectilesPerShot; i++)
        {
            // 🎯 Spread
            Quaternion spreadRot = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0f
            );

            Vector3 direction = spreadRot * firePoint.forward;

            GameObject proj = Instantiate(
                projectilePrefab,
                firePoint.position + firePoint.forward,
                Quaternion.LookRotation(direction)
            );

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(direction * projectileForce);
            }
        }

        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, firePoint.position);
        }
    }
}
