

public class BleedingEffect : StatusEffect
{
    public BleedingEffect(int stacks) : base(stacks){}
    public override void OnAfterDrawnCard(PlayerState player)
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
