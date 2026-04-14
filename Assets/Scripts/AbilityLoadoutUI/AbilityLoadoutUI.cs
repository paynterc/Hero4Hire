using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AbilityLoadoutUI : MonoBehaviour
{
    [Header("Registry")]
    public AbilityRegistry registry;

    [Header("Layout")]
    public Transform slotListContainer;
    public Transform abilityPickerContainer;

    [Header("Prefabs")]
    public GameObject slotRowPrefab;
    public GameObject abilityCardPrefab;

    [Header("UI Elements")]
    public TMP_Text resourcePointsLabel;
    public Button   confirmButton;
    public TMP_Text confirmButtonLabel;

    [Header("Scene Navigation")]
    public string nextSceneName = "";

    // ── Runtime state ──────────────────────────────────────────────────────────

    private AbilityLoadout _loadout;
    private ActionSlot     _selectedSlot     = ActionSlot.Primary;
    private bool           _showingModifiers = false;

    private readonly Dictionary<ActionSlot, AbilitySlotRow> _slotRows
        = new Dictionary<ActionSlot, AbilitySlotRow>();

    // ── Lifecycle ──────────────────────────────────────────────────────────────

    void Start()
    {
        var config = CharacterManager.Instance?.config;
        if (config == null)
        {
            Debug.LogWarning("AbilityLoadoutUI: no CharacterManager config found.");
            return;
        }

        _loadout = config.loadout;

        // Grant starting points on first visit (no slots selected yet)
        if (_loadout.slots.Count == 0 && _loadout.resourcePoints == 0)
            _loadout.resourcePoints = registry != null ? registry.startingResourcePoints : 0;

        BuildSlotRows();
        SelectSlot(ActionSlot.Primary);
        RefreshResourceLabel();

        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirm);
    }

    // ── Slot row list ──────────────────────────────────────────────────────────

    void BuildSlotRows()
    {
        if (slotListContainer == null || slotRowPrefab == null) return;

        foreach (Transform child in slotListContainer)
            Destroy(child.gameObject);
        _slotRows.Clear();

        foreach (ActionSlot slot in System.Enum.GetValues(typeof(ActionSlot)))
        {
            var go  = Instantiate(slotRowPrefab, slotListContainer);
            var row = go.GetComponent<AbilitySlotRow>();
            if (row == null) continue;

            row.Init(slot, this);
            row.SetEquipped(GetEquippedName(slot));
            _slotRows[slot] = row;
        }
    }

    // ── Slot selection & picker rebuild ───────────────────────────────────────

    public void SelectSlot(ActionSlot slot)
    {
        _selectedSlot     = slot;
        _showingModifiers = false;
        RebuildPicker();
    }

    public void ShowModifiers(ActionSlot slot)
    {
        _selectedSlot     = slot;
        _showingModifiers = true;
        RebuildPicker();
    }

    void RebuildPicker()
    {
        if (abilityPickerContainer == null || abilityCardPrefab == null) return;

        foreach (Transform child in abilityPickerContainer)
            Destroy(child.gameObject);

        if (registry == null) return;

        if (!_showingModifiers)
        {
            BuildBasePicker();
        }
        else
        {
            BuildModifierPicker();
        }
    }

    void BuildBasePicker()
    {
        var bases = registry.GetBaseAbilitiesForSlot(_selectedSlot);
        foreach (var ability in bases)
        {
            var go   = Instantiate(abilityCardPrefab, abilityPickerContainer);
            var card = go.GetComponent<AbilityCardUI>();
            if (card == null) continue;

            var captured = ability;
            card.SetupBase(captured, () => EquipBase(captured, _selectedSlot));
        }
    }

    void BuildModifierPicker()
    {
        // Need an equipped base ability to know which modifiers are compatible
        var equippedAbility = GetEquippedAbility(_selectedSlot);
        if (equippedAbility == null)
        {
            // No base ability equipped — show a message card or just leave empty
            return;
        }

        // Active modifiers for this slot first (shown as Remove cards)
        var activeModifiers = _loadout.modifiers.FindAll(
            m => m.isGlobal || m.targetSlot == _selectedSlot);

        foreach (var entry in activeModifiers)
        {
            var ability = registry.FindByName(entry.abilityName);
            if (ability == null) continue;

            var go   = Instantiate(abilityCardPrefab, abilityPickerContainer);
            var card = go.GetComponent<AbilityCardUI>();
            if (card == null) continue;

            var capturedEntry = entry;
            card.SetupRemove(ability, () => RemoveModifier(capturedEntry));
        }

        // Available modifiers filtered by the equipped ability's type
        var available = registry.GetModifiersForAbilityType(equippedAbility.abilityType);
        foreach (var ability in available)
        {
            bool alreadyActive = _loadout.modifiers.Exists(
                m => m.abilityName == ability.abilityName &&
                     (m.isGlobal || m.targetSlot == _selectedSlot));
            if (alreadyActive) continue;

            // A modifier with no validAbilityTypes is considered global across slots
            bool isGlobal  = ability.validAbilityTypes == null || ability.validAbilityTypes.Length == 0;
            bool canAfford = ability.cost == 0 || _loadout.resourcePoints >= ability.cost;

            var go   = Instantiate(abilityCardPrefab, abilityPickerContainer);
            var card = go.GetComponent<AbilityCardUI>();
            if (card == null) continue;

            var capturedAbility  = ability;
            var capturedIsGlobal = isGlobal;
            card.SetupModifier(capturedAbility, canAfford,
                () => AddModifier(capturedAbility, _selectedSlot, capturedIsGlobal));
        }
    }

    // ── Equip / modifier logic ─────────────────────────────────────────────────

    public void EquipBase(Ability ability, ActionSlot slot)
    {
        int idx = _loadout.slots.FindIndex(e => e.slot == slot);
        if (idx >= 0)
            _loadout.slots[idx].abilityName = ability.abilityName;
        else
            _loadout.slots.Add(new SlotEntry { slot = slot, abilityName = ability.abilityName });

        if (_slotRows.TryGetValue(slot, out var row))
            row.SetEquipped(ability.abilityName);

        RebuildPicker();
    }

    public void AddModifier(Ability ability, ActionSlot targetSlot, bool isGlobal)
    {
        if (ability.cost > 0)
        {
            if (_loadout.resourcePoints < ability.cost) return;
            _loadout.resourcePoints -= ability.cost;
        }

        _loadout.modifiers.Add(new ModifierEntry
        {
            abilityName = ability.abilityName,
            targetSlot  = targetSlot,
            isGlobal    = isGlobal
        });

        RefreshResourceLabel();
        RebuildPicker();
    }

    public void RemoveModifier(ModifierEntry entry)
    {
        var ability = registry?.FindByName(entry.abilityName);
        if (ability != null && ability.cost > 0)
            _loadout.resourcePoints += ability.cost;

        _loadout.modifiers.Remove(entry);

        RefreshResourceLabel();
        RebuildPicker();
    }

    // ── Confirm ────────────────────────────────────────────────────────────────

    void OnConfirm()
    {
        CharacterManager.Instance?.Save();

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    string GetEquippedName(ActionSlot slot)
    {
        var entry = _loadout.slots.Find(e => e.slot == slot);
        return entry?.abilityName ?? "";
    }

    Ability GetEquippedAbility(ActionSlot slot)
    {
        var name = GetEquippedName(slot);
        return string.IsNullOrEmpty(name) ? null : registry?.FindByName(name);
    }

    void RefreshResourceLabel()
    {
        if (resourcePointsLabel != null)
            resourcePointsLabel.text = $"Points: {_loadout.resourcePoints}";
    }
}
