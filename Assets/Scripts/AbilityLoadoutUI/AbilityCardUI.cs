using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityCardUI : MonoBehaviour
{
    public Image    icon;
    public TMP_Text nameLabel;
    public TMP_Text descLabel;
    public TMP_Text costLabel;
    public Button   actionButton;
    public TMP_Text actionButtonLabel;

    public void SetupBase(Ability ability, System.Action onEquip)
    {
        Populate(ability);
        actionButtonLabel.text = "Equip";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onEquip());
    }

    public void SetupModifier(Ability ability, bool canAfford, System.Action onAdd)
    {
        Populate(ability);
        actionButtonLabel.text = ability.cost == 0 ? "Add (Free)" : $"Add ({ability.cost} pts)";
        actionButton.interactable = canAfford;
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onAdd());
    }

    public void SetupRemove(Ability ability, System.Action onRemove)
    {
        Populate(ability);
        actionButtonLabel.text = "Remove";
        actionButton.interactable = true;
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onRemove());
    }

    void Populate(Ability ability)
    {
        if (icon != null)
        {
            icon.sprite  = ability.icon;
            icon.enabled = ability.icon != null;
        }
        if (nameLabel != null) nameLabel.text = ability.abilityName;
        if (descLabel  != null) descLabel.text  = ability.description;
        if (costLabel  != null) costLabel.text  = ability.cost == 0 ? "Free" : $"{ability.cost} pts";
    }
}
