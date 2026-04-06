using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Modifiers/Shield Explosion")]
public class ShieldExplosionAbility : Ability
{
    public GameObject explosionPrefab;
    public float explosionRadius = 6f;
    public int explosionDamage = 50;
    public LayerMask damageLayers;
    public bool explodeOnBreak = true;
    public bool explodeOnExpire = false;

    public override void OnActivateShield(GameObject owner, AbilityInstance instance, ShieldInstance shield)
    {
        if (explodeOnBreak)
            shield.OnBroke += () => Explode(owner, shield);
        if (explodeOnExpire)
            shield.OnExpired += () => Explode(owner, shield);
    }

    void Explode(GameObject owner, ShieldInstance shield)
    {
        Vector3 pos = owner.transform.position;

        if (explosionPrefab != null)
            Object.Instantiate(explosionPrefab, pos, Quaternion.identity);

        var hits = Physics.OverlapSphere(pos, explosionRadius, damageLayers);
        foreach (var hit in hits)
        {
            if (hit.gameObject == owner) continue;
            hit.GetComponentInParent<Health>()?.TakeDamage(explosionDamage, owner);
        }
    }
}
