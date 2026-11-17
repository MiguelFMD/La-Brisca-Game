

public class BleedingEffect : StatusEffect
{
    public BleedingEffect(int stacks) : base(stacks){}
    public override void OnAfterDrawnCard(PlayerState player)
    {
        DamageContext ctx = new DamageContext
        {
            Target = player,
            Amount = 1,
            DamageType = DamageType.Bleeding,
            IsRetaliation = false
        };
        ReduceStack();
        ctx.Target.TakeDamage(ctx);
    }
}
