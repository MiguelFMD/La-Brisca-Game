using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/GameEvent")]
public class GameEvent : ScriptableObject
{
    //The list of listeners that this event will notify if is raised
    private List<GameEventListener> listeners = new List<GameEventListener>();

    public void RegisterListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }

    public void Raise()
    {
        for(int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }
}

