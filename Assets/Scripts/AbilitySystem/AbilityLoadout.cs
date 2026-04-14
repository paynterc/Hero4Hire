using System;
using System.Collections.Generic;

[Serializable]
public class AbilityLoadout
{
    public int resourcePoints = 0;
    public List<SlotEntry> slots = new List<SlotEntry>();
    public List<ModifierEntry> modifiers = new List<ModifierEntry>();
}

[Serializable]
public class SlotEntry
{
    public ActionSlot slot;
    public string abilityName;
}

[Serializable]
public class ModifierEntry
{
    public string abilityName;
    public ActionSlot targetSlot;
    public bool isGlobal;
}
