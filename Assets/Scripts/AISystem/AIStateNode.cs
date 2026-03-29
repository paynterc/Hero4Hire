using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIStateNode
{
    public string name;
    [SerializeReference]
    public AIState state;
    public List<AITransition> transitions = new List<AITransition>();
}
