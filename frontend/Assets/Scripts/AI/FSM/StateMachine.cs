using System;
using System.Collections.Generic;

public class StateMachine
{
    private IState currentState;
    private readonly Dictionary<Type, IState> states = new();

    public void AddState(IState state)
    {
        states[state.GetType()] = state;
    }

    public void ChangeState<T>() where T : IState
    {
        if (states.TryGetValue(typeof(T), out var newState))
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
        else
        {
            UnityEngine.Debug.LogWarning($"[StateMachine] ���� {typeof(T).Name} ��(��) ��ϵ��� �ʾҽ��ϴ�.");
        }
    }

    public void Tick()
    {
        if (currentState == null)
        {
            UnityEngine.Debug.LogWarning("[StateMachine] ���� ���°� �����ϴ�.");
            return;
        }
        currentState.Tick();
    }
}
