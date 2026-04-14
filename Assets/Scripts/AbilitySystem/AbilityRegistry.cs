using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Registry")]
public class AbilityRegistry : ScriptableObject
{
    public List<Ability> baseAbilities = new List<Ability>();
    public List<Ability> modifiers = new List<Ability>();
    public int startingResourcePoints = 10;

    public Ability FindByName(string name)
    {
        foreach (var a in baseAbilities)
            if (a != null && a.abilityName == name) return a;
        foreach (var a in modifiers)
            if (a != null && a.abilityName == name) return a;
        return null;
    }

    public List<Ability> GetBaseAbilitiesForSlot(ActionSlot slot)
    {
        var result = new List<Ability>();
        foreach (var a in baseAbilities)
        {
            if (a == null) continue;
            if (a.validSlots == null || a.validSlots.Length == 0) continue;
            foreach (var s in a.validSlots)
                if (s == slot) { result.Add(a); break; }
        }
        return result;
    }

    public List<Ability> GetModifiersForSlot(ActionSlot slot)
    {
        var result = new List<Ability>();
        foreach (var a in modifiers)
        {
            if (a == null) continue;
            // Global modifiers (no validSlots) are always included
            if (a.validSlots == null || a.validSlots.Length == 0)
            {
                result.Add(a);
                continue;
            }
            foreach (var s in a.validSlots)
                if (s == slot) { result.Add(a); break; }
        }
        return result;
    }

    public List<AbilitySlotBinding> BuildBindings(AbilityLoadout loadout)
    {
        var bindings = new List<AbilitySlotBinding>();
        if (loadout == null) return bindings;

        foreach (var entry in loadout.slots)
        {
            var ability = FindByName(entry.abilityName);
            if (ability != null)
                bindings.Add(new AbilitySlotBinding { ability = ability, slot = entry.slot });
        }

        foreach (var entry in loadout.modifiers)
        {
            var ability = FindByName(entry.abilityName);
            if (ability == null) continue;

            if (entry.isGlobal)
            {
                // Add to every occupied slot
                var occupiedSlots = new HashSet<ActionSlot>();
                foreach (var b in bindings) occupiedSlots.Add(b.slot);
                foreach (var s in occupiedSlots)
                    bindings.Add(new AbilitySlotBinding { ability = ability, slot = s });
            }
            else
            {
                bindings.Add(new AbilitySlotBinding { ability = ability, slot = entry.targetSlot });
            }
        }

        return bindings;
    }
}
