public abstract class StatusEffect
{
    public int Stacks { get; set; }
    public Player Owner { get; set; }

    public virtual void OnTurnEnd() {}
    public virtual void OnTrickWon(TrickResult result) {}
    public virtual void OnBeforeReceiveDamage(DamageContext ctx) {}
    public virtual void OnAfterReceiveDamage(DamageContext ctx) {}
    public virtual void OnAfterDrawnCard(Player player) {}
}