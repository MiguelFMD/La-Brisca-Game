using UnityEngine;

public class PowerShield : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        base.Activate(cardData, source, target);
        source.AddShield(BriscaRules.CalculateCardValue(cardData) + powerInvested);
    }
}
