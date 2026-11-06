
public class PowerBleed : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        DamageContext damageContext = new DamageContext
        {
            Source = source,
            Target = target,
            DamageType = DamageType.Bleeding,
            IsRetaliation = false,
            Amount = BriscaRules.CalculateCardValue(cardData)
        };

        
    }
}
