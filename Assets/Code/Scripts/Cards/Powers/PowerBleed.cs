
public class PowerBleed : Power
{
    public override void Activate(CardData cardData, Player source, Player target)
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
