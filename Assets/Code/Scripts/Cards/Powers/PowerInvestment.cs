using UnityEngine;

public class PowerInvestment : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        base.Activate(cardData, source, target);
        source.AddInvestment(BriscaRules.CalculateCardValue(cardData) + powerInvested);
    }
}
