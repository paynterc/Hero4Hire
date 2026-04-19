using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AbilitySlotRow : MonoBehaviour
{
    public TMP_Text slotLabel;
    public TMP_Text equippedLabel;
    public Button   selectButton;
    public Button   modifiersButton;

    private ActionSlot     _slot;
    private AbilityLoadoutUI _ui;

    public void Init(ActionSlot slot, AbilityLoadoutUI ui)
    {
        _slot = slot;
        _ui   = ui;

        if (slotLabel != null)
            slotLabel.text = slot.ToString();

        if (selectButton != null)
            selectButton.onClick.AddListener(() => _ui.SelectSlot(_slot));

        if (modifiersButton != null)
            modifiersButton.onClick.AddListener(() => _ui.ShowModifiers(_slot));

        RefreshModifiersButton(null);
    }

    public void SetEquipped(string abilityName)
    {
        if (equippedLabel != null)
            equippedLabel.text = string.IsNullOrEmpty(abilityName) ? "— empty —" : abilityName;

        RefreshModifiersButton(abilityName);
    }

    // Modifiers button is only interactable when a base ability is equipped
    void RefreshModifiersButton(string abilityName)
    {
        if (modifiersButton != null)
            modifiersButton.interactable = !string.IsNullOrEmpty(abilityName);
    }
}
