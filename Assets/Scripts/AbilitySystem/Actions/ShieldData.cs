using UnityEngine;

public class ShieldData : IActionData
{
    public ActionSlot slot;
    public GameObject shieldPrefab;
    public float duration = 5f;
    public int shieldHealth = 100;
    public float radius = 2f;
    public float energyCost = 0f;
    public ActionContext context;
    public float yOffset;
}
