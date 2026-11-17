
public class PowerBleed : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        BleedingEffect bleedingEffect = new BleedingEffect(BriscaRules.CalculateCardValue(cardData));
        target.AddStatusEffect(bleedingEffect);
    }
}
