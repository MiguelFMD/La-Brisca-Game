
public class PowerHeal : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        base.Activate(cardData, source, target);
        float healAmount = BriscaRules.CalculateCardValue(cardData) + powerInvested;
        target.Heal(healAmount);
    }
}
