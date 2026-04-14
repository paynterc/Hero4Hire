using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AbilitySlotRow : MonoBehaviour
{
    public TMP_Text slotLabel;
    public TMP_Text equippedLabel;
    public Button button;

    private ActionSlot _slot;
    private AbilityLoadoutUI _ui;

    public void Init(ActionSlot slot, AbilityLoadoutUI ui)
    {
        _slot = slot;
        _ui   = ui;

        if (slotLabel != null)
            slotLabel.text = slot.ToString();

        if (button != null)
            button.onClick.AddListener(() => _ui.SelectSlot(_slot));
    }

    public void SetEquipped(string abilityName)
    {
        if (equippedLabel != null)
            equippedLabel.text = string.IsNullOrEmpty(abilityName) ? "— empty —" : abilityName;
    }
}
