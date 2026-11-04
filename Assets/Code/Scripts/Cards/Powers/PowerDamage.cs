
public class PowerDamage : Power
{
    public override void Activate(CardData cardData, Player source, Player target)
    {
        DamageContext damageContext = new DamageContext
        {
            Source = source,
            Target = target,
            DamageType = DamageType.Trick,
            IsRetaliation = false,
            Amount = BriscaRules.CalculateCardValue(cardData)
        };

        target.TakeDamage(damageContext.Amount);
    }
}
