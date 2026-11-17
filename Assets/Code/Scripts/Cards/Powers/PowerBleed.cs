
public class PowerBleed : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        base.Activate(cardData, source, target);
        BleedingEffect bleedingEffect = new BleedingEffect(BriscaRules.CalculateCardValue(cardData) + powerInvested);
        target.AddStatusEffect(bleedingEffect);
    }
}
