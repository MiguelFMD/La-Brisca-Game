using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameEventListener<T> : MonoBehaviour
{
    [Tooltip("Game event to listen to")]
    [SerializeField] private BaseGameEvent<T> gameEvent;
    [Tooltip("Response when GameEvent is fired/raised")]
    [SerializeField] private UnityEvent<T> response;
    
    private void OnEnable() => gameEvent?.RegisterListener(this);
    private void OnDisable() => gameEvent?.UnRegisterListener(this);
    public void OnEventRaised(T value) => response?.Invoke(value);
}
