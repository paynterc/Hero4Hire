using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class CharacterPreview : MonoBehaviour
{
    private static readonly string[] SlotFolders =
        { "Hair", "Mask", "Beard", "Helmet", "Back", "Boots", "Gloves" };

    private static readonly string[] AttachPointNames =
        { "AttachHair", "AttachMask", "AttachBeard", "AttachHelmet", "AttachBack", "AttachBoots", "AttachGloves" };

    // Slots where the second attach point should have its X scale mirrored
    private static readonly bool[] MirrorSecond =
        { false, false, false, false, false, true, true }; // Boots, Gloves mirrored

    private GameObject[]   bodyPrefabs;
    private GameObject[][] accessoryPrefabs = new GameObject[7][];
    private Material[]     decalMaterials;
    private DecalProjector decalProjector;

    private GameObject     currentBody;

    private List<GameObject>[] spawnedAccessories;
    private List<Transform>[]  attachPoints;

    void Awake()
    {
        spawnedAccessories = new List<GameObject>[7];
        attachPoints       = new List<Transform>[7];
        for (int i = 0; i < 7; i++)
        {
            spawnedAccessories[i] = new List<GameObject>();
            attachPoints[i]       = new List<Transform>();
        }

        bodyPrefabs    = Resources.LoadAll<GameObject>("Characters/Body");
        decalMaterials = Resources.LoadAll<Material>("Characters/Decals");
        for (int i = 0; i < 7; i++)
            accessoryPrefabs[i] = Resources.LoadAll<GameObject>($"Characters/{SlotFolders[i]}");

        if (bodyPrefabs.Length == 0)
        {
            currentBody  = gameObject;
            BodyAnimator = GetComponentInChildren<Animator>();
            RefreshAttachPoints();
        }
    }

    // ── Accessors for UI ─────────────────────────────────────────────────

    public GameObject[] GetBodyPrefabs()              => bodyPrefabs;
    public GameObject[] GetAccessoryPrefabs(int slot) => accessoryPrefabs[slot];

    // ── Body ─────────────────────────────────────────────────────────────

    public Animator BodyAnimator { get; private set; }

    public void SetBody(int index, CharacterConfig config)
    {
        if (currentBody != null) Destroy(currentBody);
        if (bodyPrefabs == null || index < 0 || index >= bodyPrefabs.Length) return;

        currentBody  = Instantiate(bodyPrefabs[index], transform.position, transform.rotation, transform);
        BodyAnimator = currentBody.GetComponentInChildren<Animator>();

        var relay = currentBody.GetComponentInChildren<AnimatorIKRelay>();
        if (relay != null)
            relay.playerIK = GetComponent<PlayerIK>();

        RefreshAttachPoints();
        ApplyBodyColors(config.skinColor, config.primaryColor, config.secondaryColor);

        for (int i = 0; i < 7; i++)
            SetAccessory(i, config.accessoryIndices[i], config.accessoryColors[i]);
    }

    private Color _lastSkin      = Color.white;
    private Color _lastPrimary   = Color.white;
    private Color _lastSecondary = Color.white;

    public void ApplyBodyColors(Color skin, Color primary, Color secondary)
    {
        _lastSkin      = skin;
        _lastPrimary   = primary;
        _lastSecondary = secondary;

        var root = currentBody != null ? currentBody : gameObject;
        foreach (var r in root.GetComponentsInChildren<Renderer>())
        {
            Color color;
            string n = r.gameObject.name;
            if      (n.EndsWith("_Skin"))      color = skin;
            else if (n.EndsWith("_Primary"))   color = primary;
            else if (n.EndsWith("_Secondary")) color = secondary;
            else continue;

            var block = new MaterialPropertyBlock();
            block.SetColor("_BaseColor", color);
            r.SetPropertyBlock(block);
        }
    }

    public void ApplySkinColor(Color skin)       => ApplyBodyColors(skin,       _lastPrimary,   _lastSecondary);
    public void ApplyPrimaryColor(Color color)   => ApplyBodyColors(_lastSkin,  color,          _lastSecondary);
    public void ApplySecondaryColor(Color color) => ApplyBodyColors(_lastSkin,  _lastPrimary,   color);

    // ── Attach point discovery ────────────────────────────────────────────

    void RefreshAttachPoints()
    {
        decalProjector = currentBody.GetComponentInChildren<DecalProjector>();
        if (decalProjector == null)
            Debug.LogWarning("[CharacterPreview] No DecalProjector found on body.");

        for (int i = 0; i < 7; i++)
        {
            attachPoints[i].Clear();

            int index = 0;
            while (true)
            {
                var t = FindDeep(currentBody.transform, $"{AttachPointNames[i]}_{index}");
                if (t == null) break;
                attachPoints[i].Add(t);
                index++;
            }

            if (attachPoints[i].Count == 0)
            {
                var t = FindDeep(currentBody.transform, AttachPointNames[i]);
                if (t != null)
                    attachPoints[i].Add(t);
                else
                    Debug.LogWarning($"[CharacterPreview] No attach point found for '{AttachPointNames[i]}' on body.");
            }
        }
    }

    Transform FindDeep(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var found = FindDeep(child, name);
            if (found != null) return found;
        }
        return null;
    }

    // ── Accessories ───────────────────────────────────────────────────────

    public void SetAccessory(int slotIndex, int prefabIndex, Color color)
    {
        foreach (var go in spawnedAccessories[slotIndex])
            if (go != null) Destroy(go);
        spawnedAccessories[slotIndex].Clear();

        if (prefabIndex < 0) return;

        var prefabs = accessoryPrefabs[slotIndex];
        if (prefabs == null || prefabIndex >= prefabs.Length) return;

        var points = attachPoints[slotIndex];
        if (points == null || points.Count == 0) return;

        for (int p = 0; p < points.Count; p++)
        {
            var point = points[p];
            var go    = Instantiate(prefabs[prefabIndex], point, false);

            if (p > 0 && MirrorSecond[slotIndex])
                go.transform.localScale = new Vector3(-1f, 1f, 1f);

            ApplyColor(go, color);
            spawnedAccessories[slotIndex].Add(go);
        }
    }

    public void SetAccessoryColor(int slotIndex, Color color)
    {
        foreach (var go in spawnedAccessories[slotIndex])
            if (go != null) ApplyColor(go, color);
    }

    void ApplyColor(GameObject go, Color color)
    {
        var block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", color);
        foreach (var r in go.GetComponentsInChildren<Renderer>())
            r.SetPropertyBlock(block);
    }

    // ── Decal ─────────────────────────────────────────────────────────────

    public Material[] GetDecalMaterials() => decalMaterials;

    private Material decalMaterialInstance;
    private Color    _lastDecalColor = Color.white;

    public void SetDecal(int index)
    {
        if (decalProjector == null) return;
        if (index < 0)
        {
            decalProjector.enabled = false;
            return;
        }
        if (decalMaterials == null || index >= decalMaterials.Length) return;

        if (decalMaterialInstance != null) Destroy(decalMaterialInstance);
        decalMaterialInstance = new Material(decalMaterials[index]);
        decalMaterialInstance.SetColor("_BaseColor", _lastDecalColor);

        decalProjector.enabled  = true;
        decalProjector.material = decalMaterialInstance;
    }

    public void SetDecalColor(Color color)
    {
        _lastDecalColor = color;
        if (decalMaterialInstance == null) return;
        decalMaterialInstance.SetColor("_TintColor", color);
    }

    public void SetDecalSize(float width, float height)
    {
        if (decalProjector == null) return;
        var s = decalProjector.size;
        decalProjector.size = new Vector3(width, height, s.z);
    }

    // ── Full config apply ─────────────────────────────────────────────────

    public void ApplyConfig(CharacterConfig config)
    {
        SetBody(config.bodyIndex, config);
        SetDecal(config.decalIndex);
        SetDecalColor(config.decalColor);
        SetDecalSize(config.decalWidth, config.decalHeight);
    }
}
