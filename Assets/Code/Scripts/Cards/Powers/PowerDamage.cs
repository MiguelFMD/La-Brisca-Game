
public class PowerDamage : Power
{
    public override void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        DamageContext damageContext = new DamageContext
        {
            Source = source,
            Target = target,
            DamageType = DamageType.Trick,
            IsRetaliation = false,
            Amount = BriscaRules.CalculateCardValue(cardData)
        };

        target.TakeDamage(damageContext);
    }
}
