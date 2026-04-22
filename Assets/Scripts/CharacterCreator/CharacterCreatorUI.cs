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

    [Header("Color Picker")]
    public FlexibleColorPicker colorPicker;

    [Header("Color Target Buttons")]
    [Tooltip("Button whose Image shows the current skin color")]
    public Button skinColorButton;
    [Tooltip("Button whose Image shows the current primary color")]
    public Button primaryColorButton;
    [Tooltip("Button whose Image shows the current secondary color")]
    public Button secondaryColorButton;
    [Tooltip("Button whose Image shows the current accessory color")]
    public Button accessoryColorButton;
    [Tooltip("Button whose Image shows the current decal color")]
    public Button decalColorButton;

    [Header("Color Button Groups")]
    [Tooltip("Parent of skin/primary/secondary buttons — shown when body is active")]
    public GameObject bodyColorButtons;
    [Tooltip("Parent of accessory button — shown when an accessory slot is active")]
    public GameObject accessoryColorButtons;
    [Tooltip("Parent of decal button — shown when decal is active")]
    public GameObject decalColorButtons;

    [Header("UI Elements")]
    public TMP_InputField nameInput;
    public TMP_Text       nameDisplay;
    public Button         saveButton;

    [Header("Decal")]
    public float decalSizeStep = 0.05f;
    public float decalSizeMin  = 0.1f;
    public float decalSizeMax  = 2f;

    // ── Internal ──────────────────────────────────────────────────────────

    private static readonly string[] SlotLabels =
        { "Body", "Hair", "Mask", "Beard", "Helmet", "Back", "Boots", "Gloves" };

    private CharacterConfig config;
    private int activeSlot = -1; // -1 = body, 0-6 = accessory slot

    enum ColorTarget { None, Skin, Primary, Secondary, Accessory, Decal }
    ColorTarget activeColorTarget = ColorTarget.None;

    // Listener stored so we can remove it before wiring a new target
    UnityEngine.Events.UnityAction<Color> colorChangeListener;

    // ── Lifecycle ─────────────────────────────────────────────────────────

    void Start()
    {
        config = CharacterManager.Instance != null
            ? CharacterManager.Instance.config
            : new CharacterConfig();

        BuildRows();
        ShowPalettesFor(PaletteMode.Body);

        nameInput.text = config.characterName;
        if (nameDisplay != null) nameDisplay.text = config.characterName;
        nameInput.onValueChanged.AddListener(v =>
        {
            config.characterName = v;
            if (nameDisplay != null) nameDisplay.text = v;
        });
        saveButton.onClick.AddListener(OnSave);

        // Wire color target buttons
        skinColorButton.onClick.AddListener(()      => SetColorTarget(ColorTarget.Skin));
        primaryColorButton.onClick.AddListener(()   => SetColorTarget(ColorTarget.Primary));
        secondaryColorButton.onClick.AddListener(() => SetColorTarget(ColorTarget.Secondary));
        accessoryColorButton.onClick.AddListener(() => SetColorTarget(ColorTarget.Accessory));
        decalColorButton.onClick.AddListener(()     => SetColorTarget(ColorTarget.Decal));

        preview.ApplyConfig(config);
        UpdateColorButtonImages();

        // Default to skin color on open
        SetColorTarget(ColorTarget.Skin);
    }

    // ── Color target ──────────────────────────────────────────────────────

    void SetColorTarget(ColorTarget target)
    {
        activeColorTarget = target;

        // Detach previous listener
        if (colorChangeListener != null)
        {
            colorPicker.onColorChange.RemoveListener(colorChangeListener);
            colorChangeListener = null;
        }

        // Find the current color for this target
        Color current = GetConfigColor(target);
        colorPicker.color = current;

        // Wire new listener
        colorChangeListener = color =>
        {
            SetConfigColor(target, color);
            UpdateColorButtonImages();
        };
        colorPicker.onColorChange.AddListener(colorChangeListener);
    }

    Color GetConfigColor(ColorTarget target)
    {
        switch (target)
        {
            case ColorTarget.Skin:      return config.skinColor;
            case ColorTarget.Primary:   return config.primaryColor;
            case ColorTarget.Secondary: return config.secondaryColor;
            case ColorTarget.Accessory: return activeSlot >= 0 ? config.accessoryColors[activeSlot] : Color.white;
            case ColorTarget.Decal:     return config.decalColor;
            default:                    return Color.white;
        }
    }

    void SetConfigColor(ColorTarget target, Color color)
    {
        switch (target)
        {
            case ColorTarget.Skin:
                config.skinColor = color;
                preview.ApplySkinColor(color);
                break;
            case ColorTarget.Primary:
                config.primaryColor = color;
                preview.ApplyPrimaryColor(color);
                break;
            case ColorTarget.Secondary:
                config.secondaryColor = color;
                preview.ApplySecondaryColor(color);
                break;
            case ColorTarget.Accessory:
                if (activeSlot >= 0)
                {
                    config.accessoryColors[activeSlot] = color;
                    preview.SetAccessoryColor(activeSlot, color);
                }
                break;
            case ColorTarget.Decal:
                config.decalColor = color;
                preview.SetDecalColor(color);
                break;
        }
    }

    void UpdateColorButtonImages()
    {
        SetButtonColor(skinColorButton,       config.skinColor);
        SetButtonColor(primaryColorButton,    config.primaryColor);
        SetButtonColor(secondaryColorButton,  config.secondaryColor);
        if (activeSlot >= 0)
            SetButtonColor(accessoryColorButton, config.accessoryColors[activeSlot]);
        SetButtonColor(decalColorButton, config.decalColor);
    }

    void SetButtonColor(Button btn, Color color)
    {
        if (btn == null) return;
        color.a = 1f;
        btn.GetComponent<Image>().color = color;
    }

    // ── Row builder ───────────────────────────────────────────────────────

    void BuildRows()
    {
        for (int i = 0; i < SlotLabels.Length; i++)
        {
            int slot = i - 1;
            AddRow(SlotLabels[i],
                () => SelectSlot(slot),
                () => Step(slot, -1),
                () => Step(slot, +1));
        }

        AddRow("Decal",
            () => SelectDecal(),
            () => StepDecal(-1),
            () => StepDecal(+1));

        AddRow("Decal W",
            () => SelectDecal(),
            () => StepDecalWidth(-decalSizeStep),
            () => StepDecalWidth(+decalSizeStep));

        AddRow("Decal H",
            () => SelectDecal(),
            () => StepDecalHeight(-decalSizeStep),
            () => StepDecalHeight(+decalSizeStep));
    }

    void AddRow(string label, System.Action onSelect, System.Action onBack, System.Action onForward)
    {
        var row     = Instantiate(rowPrefab, rowContainer);
        var texts   = row.GetComponentsInChildren<TMP_Text>();
        var buttons = row.GetComponentsInChildren<Button>();
        texts[0].text = label;
        buttons[0].onClick.AddListener(() => onSelect());   // label button
        buttons[1].onClick.AddListener(() => onBack());
        buttons[2].onClick.AddListener(() => onForward());
    }

    // ── Palette visibility ────────────────────────────────────────────────

    enum PaletteMode { Body, Accessory, Decal }

    void ShowPalettesFor(PaletteMode mode)
    {
        if (bodyColorButtons != null)
            bodyColorButtons.SetActive(mode == PaletteMode.Body);
        if (accessoryColorButtons != null)
            accessoryColorButtons.SetActive(mode == PaletteMode.Accessory);
        if (decalColorButtons != null)
            decalColorButtons.SetActive(mode == PaletteMode.Decal);
    }

    // ── Select slot for coloring (no step) ───────────────────────────────

    void SelectSlot(int slot)
    {
        activeSlot = slot;
        if (slot < 0)
        {
            ShowPalettesFor(PaletteMode.Body);
            SetColorTarget(ColorTarget.Skin);
        }
        else
        {
            ShowPalettesFor(PaletteMode.Accessory);
            SetColorTarget(ColorTarget.Accessory);
        }
    }

    void SelectDecal()
    {
        ShowPalettesFor(PaletteMode.Decal);
        SetColorTarget(ColorTarget.Decal);
    }

    // ── Step forward / back ───────────────────────────────────────────────

    void Step(int slot, int direction)
    {
        activeSlot = slot;
        PaletteMode mode = slot < 0 ? PaletteMode.Body : PaletteMode.Accessory;
        ShowPalettesFor(mode);

        // Switch color target to match active slot
        if (slot < 0)
            SetColorTarget(ColorTarget.Skin);
        else
            SetColorTarget(ColorTarget.Accessory);

        if (slot < 0)
        {
            var bodies = preview.GetBodyPrefabs();
            if (bodies == null || bodies.Length == 0) return;
            config.bodyIndex = Wrap(config.bodyIndex + direction, bodies.Length);
            preview.SetBody(config.bodyIndex, config);
        }
        else
        {
            var prefabs = preview.GetAccessoryPrefabs(slot);
            int count   = prefabs != null ? prefabs.Length : 0;

            int current = config.accessoryIndices[slot] + 1;
            current = Wrap(current + direction, count + 1);
            config.accessoryIndices[slot] = current - 1;

            preview.SetAccessory(slot, config.accessoryIndices[slot], config.accessoryColors[slot]);
        }
    }

    void StepDecal(int direction)
    {
        ShowPalettesFor(PaletteMode.Decal);
        SetColorTarget(ColorTarget.Decal);

        var materials = preview.GetDecalMaterials();
        int count     = materials != null ? materials.Length : 0;
        int current   = config.decalIndex + 1;
        current = Wrap(current + direction, count + 1);
        config.decalIndex = current - 1;
        preview.SetDecal(config.decalIndex);
        preview.SetDecalColor(config.decalColor);
    }

    void StepDecalWidth(float delta)
    {
        config.decalWidth = Mathf.Clamp(config.decalWidth + delta, decalSizeMin, decalSizeMax);
        preview.SetDecalSize(config.decalWidth, config.decalHeight);
    }

    void StepDecalHeight(float delta)
    {
        config.decalHeight = Mathf.Clamp(config.decalHeight + delta, decalSizeMin, decalSizeMax);
        preview.SetDecalSize(config.decalWidth, config.decalHeight);
    }

    int Wrap(int value, int length)
    {
        if (length <= 0) return 0;
        return ((value % length) + length) % length;
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
}
