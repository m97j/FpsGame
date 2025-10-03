using System;
using System.Collections.Generic;

public class Blackboard
{
    private readonly Dictionary<string, object> data = new(); // IDE0044: Made 'data' readonly, IDE0090: Simplified object creation

    public void Set<T>(string key, T value) => data[key] = value;
    public T Get<T>(string key) => data.ContainsKey(key) ? (T)data[key] : default;
    public bool Has(string key) => data.ContainsKey(key);
}

public class BlackboardConditionNode : BTNode
{
    private readonly Blackboard blackboard; // IDE0044: Made 'blackboard' readonly
    private readonly string key; // IDE0044: Made 'key' readonly
    private readonly Func<object, bool> condition; // IDE0044: Made 'condition' readonly

    public BlackboardConditionNode(Blackboard blackboard, string key, Func<object, bool> condition)
    {
        this.blackboard = blackboard;
        this.key = key;
        this.condition = condition;
    }

    public override State Tick()
    {
        if (!blackboard.Has(key)) return State.Failure;

        var value = blackboard.Get<object>(key);
        return condition(value) ? State.Success : State.Failure;
    }
}
