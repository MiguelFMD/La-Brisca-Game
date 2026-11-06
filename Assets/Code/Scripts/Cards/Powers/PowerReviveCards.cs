using UnityEngine;

public class PowerReviveCards :Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        source.Deck.ReviveCards(BriscaRules.CalculateCardValue(cardData));
    }
}
