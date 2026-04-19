using System;
using UnityEngine;

public enum AccessorySlot { Hair = 0, Mask = 1, Beard = 2, Helmet = 3, Back = 4, Boots = 5, Gloves = 6 }

[Serializable]
public class CharacterConfig
{
    public string characterName = "Hero";
    public int bodyIndex = 0;
    public int[]   accessoryIndices = new int[7];
    public Color   skinColor        = Color.white;
    public Color   primaryColor     = Color.white;
    public Color   secondaryColor   = Color.white;
    public Color[] accessoryColors  = new Color[7];
    public Color   decalColor       = Color.white;
    public int     decalIndex       = -1;
    public float   decalWidth       = 0.5f;
    public float   decalHeight      = 0.5f;
    public string  portraitPath     = "";
    public AbilityLoadout loadout = new AbilityLoadout();

    public CharacterConfig()
    {
        accessoryIndices = new int[7];
        accessoryColors  = new Color[7];
        for (int i = 0; i < 7; i++)
        {
            accessoryIndices[i] = -1;
            accessoryColors[i]  = Color.white;
        }
    }
}
