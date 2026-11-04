using UnityEngine;

public class PowerShield : Power
{
    public override void Activate(CardData cardData, Player source, Player target)
    {
        source.state.CurrentShield += BriscaRules.CalculateCardValue(cardData);
    }
}
