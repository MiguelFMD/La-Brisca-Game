public abstract class StatusEffect
{
    public int Stacks { get; private set; }
    private PlayerState Owner { get; set; }

    public StatusEffect(int stacks)
    {
        Stacks = stacks;
    }

    public void AddStacks(int amount)
    {
        Stacks += amount;
    }

    public void ClearStacks()
    {
        Stacks = 0;
    }

    public void ReduceStack()
    {
        Stacks--;
    }

    public virtual void OnTurnEnd() {}
    public virtual void OnTrickWon(TrickResult result) {}
    public virtual void OnBeforeReceiveDamage(DamageContext ctx) {}
    public virtual void OnAfterReceiveDamage(DamageContext ctx) {}
    public virtual void OnAfterDrawnCard(PlayerState player) {}
}