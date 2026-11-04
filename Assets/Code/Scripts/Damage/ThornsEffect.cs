
public class ThornsEffect : StatusEffect
{
    public override void OnAfterReceiveDamage(DamageContext ctx)
    {
        if(ctx.IsRetaliation) return;

        int reflectDamage = this.Stacks;

        ctx.Target.TakeDamage(reflectDamage);

    }
}
