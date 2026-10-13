using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    //Game event to listen to
    public GameEvent Event;
    //Response when GameEvent is fired/raised
    public UnityEvent Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnRegisterListener(this);
    }
    public void OnEventRaised()
    {
        Response.Invoke();
    }

}
