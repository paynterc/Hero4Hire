using UnityEngine;
using System.Collections.Generic;

public class ActionContext
{
    public GameObject owner;
    public AbilityInstance sourceAbility;

    public List<AbilityInstance> modifiers = new();

    public float timestamp;
}

