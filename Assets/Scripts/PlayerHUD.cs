using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player")]
    public Health health;
    public Energy energy;

    [Header("Character Info")]
    public string characterName = "Hero";
    public Sprite characterPortrait;

    [Header("Colors")]
    public Color healthColor = new Color(0.8f, 0.15f, 0.15f);
    public Color energyColor = new Color(0.15f, 0.5f, 0.9f);
    public Color barBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    public Color panelColor = new Color(0f, 0f, 0f, 0.5f);

    private RectTransform healthFillRect;
    private RectTransform energyFillRect;

    void Awake()
    {
        if (health == null)
            health = FindFirstObjectByType<Health>();

        if (energy == null)
            energy = FindFirstObjectByType<Energy>();

        BuildHUD();
    }

    void Update()
    {
        if (health != null && healthFillRect != null)
            healthFillRect.anchorMax = new Vector2((float)health.currentHealth / health.maxHealth, 1f);

        if (energy != null && energyFillRect != null)
            energyFillRect.anchorMax = new Vector2(energy.currentEnergy / energy.maxEnergy, 1f);
    }

    void BuildHUD()
    {
        RectTransform panel = CreateRect("HUD_Panel", transform as RectTransform);
        SetAnchors(panel, Vector2.zero, Vector2.zero, new Vector2(10f, 10f), new Vector2(300f, 110f));
        AddImage(panel, panelColor);

        // Portrait
        RectTransform portrait = CreateRect("Portrait", panel);
        SetAnchors(portrait, Vector2.zero, Vector2.zero, new Vector2(5f, 5f), new Vector2(100f, 100f));
        var portraitImg = AddImage(portrait, Color.gray);
        if (characterPortrait != null)
        {
            portraitImg.sprite = characterPortrait;
            portraitImg.preserveAspect = true;
        }

        // Right column
        RectTransform rightCol = CreateRect("RightColumn", panel);
        SetAnchors(rightCol, Vector2.zero, Vector2.one, new Vector2(115f, 5f), new Vector2(-10f, -5f));

        // Character name
        RectTransform nameRect = CreateRect("CharacterName", rightCol);
        SetAnchors(nameRect, new Vector2(0f, 1f), Vector2.one, new Vector2(0f, -30f), new Vector2(0f, 0f));
        var nameText = nameRect.gameObject.AddComponent<TextMeshProUGUI>();
        nameText.text = characterName;
        nameText.fontSize = 18;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = Color.white;
        nameText.alignment = TextAlignmentOptions.MidlineLeft;

        // Health bar
        healthFillRect = BuildBar("HealthBar", rightCol, new Vector2(0f, 0.6f), new Vector2(1f, 0.85f), healthColor);
        BuildLabel("HP", rightCol, new Vector2(0f, 0.6f), new Vector2(1f, 0.85f));

        // Energy bar
        energyFillRect = BuildBar("EnergyBar", rightCol, new Vector2(0f, 0.25f), new Vector2(1f, 0.5f), energyColor);
        BuildLabel("EP", rightCol, new Vector2(0f, 0.25f), new Vector2(1f, 0.5f));

        // Restart button — top-right corner of screen
        RectTransform btnRect = CreateRect("RestartButton", transform as RectTransform);
        SetAnchors(btnRect, Vector2.one, Vector2.one, new Vector2(-110f, -50f), new Vector2(-10f, -10f));
        var btnImg = AddImage(btnRect, new Color(0.2f, 0.2f, 0.2f, 0.8f));
        var btn = btnRect.gameObject.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));

        var btnLabelRect = CreateRect("RestartLabel", btnRect);
        SetAnchors(btnLabelRect, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        var btnText = btnLabelRect.gameObject.AddComponent<TextMeshProUGUI>();
        btnText.text = "Restart";
        btnText.fontSize = 16;
        btnText.fontStyle = FontStyles.Bold;
        btnText.color = Color.white;
        btnText.alignment = TextAlignmentOptions.Center;
    }

    RectTransform BuildBar(string name, RectTransform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        RectTransform bg = CreateRect(name + "_BG", parent);
        SetAnchors(bg, anchorMin, anchorMax, new Vector2(30f, 2f), new Vector2(-2f, -2f));
        AddImage(bg, barBackgroundColor);

        // Fill sits inside bg, anchored left — anchorMax.x drives the fill amount
        RectTransform fill = CreateRect(name + "_Fill", bg);
        SetAnchors(fill, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        AddImage(fill, color);
        return fill;
    }

    void BuildLabel(string text, RectTransform parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        RectTransform rect = CreateRect(text + "_Label", parent);
        SetAnchors(rect, anchorMin, anchorMax, new Vector2(2f, 2f), new Vector2(28f, -2f));
        var tmp = rect.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 11;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Midline;
    }

    RectTransform CreateRect(string name, RectTransform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.AddComponent<RectTransform>();
    }

    void SetAnchors(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
    }

    Image AddImage(RectTransform rt, Color color)
    {
        var img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        return img;
    }
}
