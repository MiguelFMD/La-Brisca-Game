

public class BleedingEffect : StatusEffect
{
    public override void OnAfterDrawnCard(Player player)
    {
        DamageContext ctx = new DamageContext
        {
            Target = player,
            Amount = this.Stacks,
            DamageType = DamageType.Bleeding,
            IsRetaliation = false
        };
        ctx.Target.TakeDamage(ctx);
    }
}
