using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class BaseGameEvent<T> : ScriptableObject
{
    //The list of listeners that this event will notify if is raised
    private readonly List<BaseGameEventListener<T>> listeners = new List<BaseGameEventListener<T>>();

    public event Action<T> OnRaised;

    public void RegisterListener(BaseGameEventListener<T> listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(BaseGameEventListener<T> listener)
    {
        listeners.Remove(listener);
    }

    public void Raise(T value)
    {
        for(int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
        OnRaised?.Invoke(value);
    }
}

