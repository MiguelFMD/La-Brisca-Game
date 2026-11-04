
public class PowerHeal : Power
{
    public override void Activate(CardData cardData, Player source, Player target)
    {
        float healAmount = BriscaRules.CalculateCardValue(cardData);
        target.Heal(healAmount);
    }
}
