using UnityEngine;

public class PowerBurnCards : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        target.Deck.BurnCards(BriscaRules.CalculateCardValue(cardData));
    }
}
