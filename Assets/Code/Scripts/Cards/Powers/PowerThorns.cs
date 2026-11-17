
public class PowerThorns : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        ThornsEffect thornsEffect = new ThornsEffect(BriscaRules.CalculateCardValue(cardData));
        target.AddStatusEffect(thornsEffect);
    }
}
