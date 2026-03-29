using System;

[Serializable]
public abstract class AIState
{
    public virtual void OnEnter(AIContext ctx) { }
    public virtual void OnUpdate(AIContext ctx) { }
    public virtual void OnExit(AIContext ctx) { }
}
