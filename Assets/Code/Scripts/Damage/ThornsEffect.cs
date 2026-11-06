
public class ThornsEffect : StatusEffect
{
    public override void OnAfterReceiveDamage(DamageContext ctx)
    {
        if(ctx.IsRetaliation) return;
        DamageContext newCtx = new DamageContext
        {
            Amount = Stacks,
            IsRetaliation = true,
            Target = ctx.Source,
            Source = ctx.Target,
            DamageType = DamageType.Thorns
        };
        ctx.Target.TakeDamage(newCtx);
    }
}
