
public class PowerThorns : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        base.Activate(cardData, source, target);
        ThornsEffect thornsEffect = new ThornsEffect(BriscaRules.CalculateCardValue(cardData) + powerInvested);
        source.AddStatusEffect(thornsEffect);
    }
}
