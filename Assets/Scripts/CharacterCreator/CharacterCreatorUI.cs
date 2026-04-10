using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreatorUI : MonoBehaviour
{
    [Header("Scene References")]
    public CharacterPreview preview;
    public PortraitCapture  portraitCapture;

    [Header("Row Prefab")]
    [Tooltip("Prefab with three children: TMP_Text (label), Button (back), Button (forward)")]
    public GameObject rowPrefab;

    [Header("UI Containers")]
    public Transform rowContainer;
    public Transform accessoryPaletteContent;
    public Transform skinPaletteContent;
    public Transform primaryPaletteContent;
    public Transform secondaryPaletteContent;

    [Header("UI Elements")]
    public TMP_InputField nameInput;
    public Button         saveButton;

    [Header("Palettes")]
    public Color[] skinPalette;
    public Color[] primaryPalette;
    public Color[] secondaryPalette;
    public Color[] accessoryPalette;

    // ── Internal ──────────────────────────────────────────────────────────

    private static readonly string[] SlotLabels =
        { "Body", "Hair", "Mask", "Beard", "Helmet", "Back", "Boots", "Gloves" };

    private CharacterConfig config;
    private int activeSlot = -1; // -1 = body, 0-6 = accessory slot

    // ── Lifecycle ─────────────────────────────────────────────────────────

    void Start()
    {
        config = CharacterManager.Instance != null
            ? CharacterManager.Instance.config
            : new CharacterConfig();

        BuildRows();
        BuildSkinPalette();
        BuildPrimaryPalette();
        BuildSecondaryPalette();
        BuildAccessoryPalette();
        primaryPaletteContent.gameObject.SetActive(true);
        secondaryPaletteContent.gameObject.SetActive(true);

        nameInput.text = config.characterName;
        nameInput.onValueChanged.AddListener(v => config.characterName = v);
        saveButton.onClick.AddListener(OnSave);

        preview.ApplyConfig(config, skinPalette, primaryPalette, secondaryPalette, accessoryPalette);
    }

    // ── Row builder ───────────────────────────────────────────────────────

    void BuildRows()
    {
        // SlotLabels[0] = Body (slot -1), SlotLabels[1..7] = accessory slots 0..6
        for (int i = 0; i < SlotLabels.Length; i++)
        {
            int slot = i - 1; // -1 for body, 0-6 for accessories

            var row      = Instantiate(rowPrefab, rowContainer);
            var texts    = row.GetComponentsInChildren<TMP_Text>();
            var buttons  = row.GetComponentsInChildren<Button>();

            // Expects prefab layout: [Label TMP_Text] [Back Button] [Forward Button]
            texts[0].text = SlotLabels[i];

            buttons[0].onClick.AddListener(() => Step(slot, -1));
            buttons[1].onClick.AddListener(() => Step(slot, +1));
        }
    }

    // ── Step forward / back ───────────────────────────────────────────────

    void Step(int slot, int direction)
    {
        activeSlot = slot;
        bool isBody = slot < 0;
        primaryPaletteContent.gameObject.SetActive(isBody);
        secondaryPaletteContent.gameObject.SetActive(isBody);
        BuildAccessoryPalette();

        if (slot < 0)
        {
            // Body
            var bodies = preview.GetBodyPrefabs();
            if (bodies == null || bodies.Length == 0) return;
            config.bodyIndex = Wrap(config.bodyIndex + direction, bodies.Length);
            preview.SetBody(config.bodyIndex, SafeSkinColor(), SafePrimaryColor(), SafeSecondaryColor(), config, accessoryPalette);
        }
        else
        {
            // Accessory — index -1 means None, 0..n-1 are prefabs
            var prefabs = preview.GetAccessoryPrefabs(slot);
            int count   = prefabs != null ? prefabs.Length : 0;

            // Shift -1..count-1 into 0..count, wrap, then shift back
            int current = config.accessoryIndices[slot] + 1;
            current = Wrap(current + direction, count + 1);
            config.accessoryIndices[slot] = current - 1;

            preview.SetAccessory(slot, config.accessoryIndices[slot], SafeAccessoryColor(slot));
        }
    }

    // Wraps value into [0, length)
    int Wrap(int value, int length)
    {
        if (length <= 0) return 0;
        return ((value % length) + length) % length;
    }

    // ── Palettes ──────────────────────────────────────────────────────────

    void BuildSkinPalette()
    {
        foreach (Transform t in skinPaletteContent) Destroy(t.gameObject);
        for (int i = 0; i < skinPalette.Length; i++)
        {
            int idx = i;
            BuildSwatch(skinPaletteContent, skinPalette[idx], () =>
            {
                config.skinColorIndex = idx;
                preview.ApplySkinColor(skinPalette[idx]);
            });
        }
    }

    void BuildPrimaryPalette()
    {
        foreach (Transform t in primaryPaletteContent) Destroy(t.gameObject);
        for (int i = 0; i < primaryPalette.Length; i++)
        {
            int idx = i;
            BuildSwatch(primaryPaletteContent, primaryPalette[idx], () =>
            {
                config.primaryColorIndex = idx;
                preview.ApplyPrimaryColor(primaryPalette[idx]);
            });
        }
    }

    void BuildSecondaryPalette()
    {
        foreach (Transform t in secondaryPaletteContent) Destroy(t.gameObject);
        for (int i = 0; i < secondaryPalette.Length; i++)
        {
            int idx = i;
            BuildSwatch(secondaryPaletteContent, secondaryPalette[idx], () =>
            {
                config.secondaryColorIndex = idx;
                preview.ApplySecondaryColor(secondaryPalette[idx]);
            });
        }
    }

    void BuildAccessoryPalette()
    {
        foreach (Transform t in accessoryPaletteContent) Destroy(t.gameObject);

        // No accessory palette when body is active
        if (activeSlot < 0) return;

        int slot = activeSlot;
        for (int i = 0; i < accessoryPalette.Length; i++)
        {
            int idx = i;
            BuildSwatch(accessoryPaletteContent, accessoryPalette[idx], () =>
            {
                config.accessoryColorIndices[slot] = idx;
                preview.SetAccessoryColor(slot, accessoryPalette[idx]);
            });
        }
    }

    void BuildSwatch(Transform container, Color color, System.Action onClick)
    {
        var btn = Instantiate(GetSwatchPrefab(), container);
        // Force alpha to 1 — inspector Color arrays default new entries to alpha 0
        color.a = 1f;
        btn.GetComponent<Image>().color = color;
        btn.GetComponent<Button>().onClick.AddListener(() => onClick());
    }

    // ── Save ──────────────────────────────────────────────────────────────

    void OnSave()
    {
        config.characterName = nameInput.text;

        var manager = CharacterManager.Instance;
        if (manager != null)
        {
            if (portraitCapture != null)
            {
                portraitCapture.SavePortrait(manager.PortraitPath);
                config.portraitPath = manager.PortraitPath;
            }
            manager.Save();
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    Color SafeSkinColor()
    {
        int idx = config.skinColorIndex;
        return skinPalette != null && idx < skinPalette.Length ? skinPalette[idx] : Color.white;
    }

    Color SafePrimaryColor()
    {
        int idx = config.primaryColorIndex;
        return primaryPalette != null && idx < primaryPalette.Length ? primaryPalette[idx] : Color.white;
    }

    Color SafeSecondaryColor()
    {
        int idx = config.secondaryColorIndex;
        return secondaryPalette != null && idx < secondaryPalette.Length ? secondaryPalette[idx] : Color.white;
    }

    Color SafeAccessoryColor(int slot)
    {
        int idx = config.accessoryColorIndices[slot];
        return accessoryPalette != null && idx < accessoryPalette.Length ? accessoryPalette[idx] : Color.white;
    }

    [Header("Swatch")]
    public float swatchSize = 40f;
    public Sprite swatchSprite;

    // Swatch prefab: a GameObject with Image + Button components
    GameObject swatchPrefabInstance;
    GameObject GetSwatchPrefab()
    {
        if (swatchPrefabInstance != null) return swatchPrefabInstance;
        swatchPrefabInstance = new GameObject("Swatch", typeof(Image), typeof(Button));
        var img = swatchPrefabInstance.GetComponent<Image>();
        img.sprite = swatchSprite;
        img.type   = Image.Type.Simple;
        var layout = swatchPrefabInstance.AddComponent<LayoutElement>();
        layout.preferredWidth  = swatchSize;
        layout.preferredHeight = swatchSize;
        return swatchPrefabInstance;
    }
}
