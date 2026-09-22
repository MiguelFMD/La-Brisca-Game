using System;

public static class EventManager
{
    public static event Action<CardDisplay> OnCardButtonClicked;
    public static event Action OnTrickEnded;
    public static event Action OnAnimationEnded;
    //public static event Action OnGameEnded;

    public static void CardButtonClicked(CardDisplay cardDisplay)
    {
        OnCardButtonClicked?.Invoke(cardDisplay);
    }

    public static void TrickEnded()
    {
        OnTrickEnded?.Invoke();
    }

    public static void AnimationEnded()
    {
        OnAnimationEnded?.Invoke();
    }
    /*public static void GameEnded()
    {
        OnGameEnded?.Invoke();
    }*/
}
