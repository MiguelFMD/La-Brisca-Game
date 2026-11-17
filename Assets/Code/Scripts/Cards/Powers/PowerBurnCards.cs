using UnityEngine;

public class PowerBurnCards : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        base.Activate(cardData, source, target);
        target.Deck.BurnCards(BriscaRules.CalculateCardValue(cardData) + powerInvested);
    }
}
