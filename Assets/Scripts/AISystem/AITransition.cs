using UnityEngine;

[System.Serializable]
public class AITransition
{
    [SerializeReference]
    public AICondition condition;
    public bool invert;
    public string targetNodeName;

    public bool Evaluate(AIContext ctx)
    {
        if (condition == null) return false;
        bool result = condition.Evaluate(ctx);
        return invert ? !result : result;
    }
}
