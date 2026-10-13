using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/GameEvent")]
public abstract class GameEvent<T> : ScriptableObject
{
    //The list of listeners that this event will notify if is raised
    private readonly List<GameEventListener<T>> listeners = new List<GameEventListener<T>>();

    public void RegisterListener(GameEventListener<T> listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(GameEventListener<T> listener)
    {
        listeners.Remove(listener);
    }

    public void Raise(T value)
    {
        for(int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised(value);
        }
    }
}

