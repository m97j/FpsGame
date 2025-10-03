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
            UnityEngine.Debug.LogWarning($"[StateMachine] 상태 {typeof(T).Name} 이(가) 등록되지 않았습니다.");
        }
    }

    public void Tick()
    {
        if (currentState == null)
        {
            UnityEngine.Debug.LogWarning("[StateMachine] 현재 상태가 없습니다.");
            return;
        }
        currentState.Tick();
    }
}
