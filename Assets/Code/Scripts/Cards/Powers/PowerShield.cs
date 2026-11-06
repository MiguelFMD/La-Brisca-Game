using UnityEngine;

public class PowerShield : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        source.CurrentShield += BriscaRules.CalculateCardValue(cardData);
    }
}
