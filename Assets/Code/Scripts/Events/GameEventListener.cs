using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [Tooltip("Game event to listen to")]
    [SerializeField] private GameEvent gameEvent;
    [Tooltip("Response when GameEvent is fired/raised")]
    [SerializeField] private UnityEvent response;
    
    private void OnEnable() => gameEvent?.RegisterListener(this);
    private void OnDisable() => gameEvent?.UnRegisterListener(this);
    public void OnEventRaised() => response?.Invoke();
}
