using UnityEngine;

public class AbilityInstance
{
    public Ability ability;
    public ActionSlot slot;

    private float cooldownTimer = 0f;
    public float fireTimer = 0f;

    public AbilityInstance(Ability ability, ActionSlot slot)
    {
        this.ability = ability;
        this.slot = slot;
    }

    public void Update(GameObject owner)
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (fireTimer > 0f)
            fireTimer -= Time.deltaTime;

        ability.OnUpdate(owner, this);
    }

    public bool IsReady()
    {
        return cooldownTimer <= 0f;
    }

    public bool CanFire()
    {
        return fireTimer <= 0f;
    }

    public void TriggerCooldown(float cooldown)
    {
        cooldownTimer = cooldown;
    }

    public void TriggerFireRate(float rate)
    {
        fireTimer = rate;
    }
}


