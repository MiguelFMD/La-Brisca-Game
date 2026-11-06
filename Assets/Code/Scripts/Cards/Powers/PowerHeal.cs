
public class PowerHeal : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        float healAmount = BriscaRules.CalculateCardValue(cardData);
        target.Heal(healAmount);
    }
}
