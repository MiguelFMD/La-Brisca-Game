
using System;

public static class EventManager
{
    public static Action<CardDisplay> OnCardButtonClicked;
    public static Action OnTrickEnded;
    public static Action OnAnimationEnded;
    public static Action OnPlayerDead;
}
