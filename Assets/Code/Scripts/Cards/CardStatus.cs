using UnityEngine;

public class CardStatus
{
    public CardData cardData {get; private set;}
    public PlayerState owner;

    public CardStatus(CardData newCardData, PlayerState newOwner)
    {
        cardData = newCardData;
        owner = newOwner;
    }
}
