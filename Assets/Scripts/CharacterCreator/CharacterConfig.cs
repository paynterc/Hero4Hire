using System;
using UnityEngine;

public enum AccessorySlot { Hair = 0, Mask = 1, Beard = 2, Helmet = 3, Back = 4, Boots = 5, Gloves = 6 }

[Serializable]
public class CharacterConfig
{
    public string characterName = "Hero";
    public int bodyIndex = 0;
    public int[] accessoryIndices  = new int[7];
    public int[] accessoryColorIndices = new int[7];
    public int skinColorIndex      = 0;
    public int primaryColorIndex   = 0;
    public int secondaryColorIndex = 0;
    public string portraitPath = "";

    public CharacterConfig()
    {
        accessoryIndices = new int[7];
        accessoryColorIndices = new int[7];
        for (int i = 0; i < 7; i++)
            accessoryIndices[i] = -1;
    }
}
