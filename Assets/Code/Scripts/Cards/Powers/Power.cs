
using System.Diagnostics;

public abstract class Power: IPowerEffect
{
    public string powerName {get; set;}
    public string powerDescription {get; set;}
    public string Name => powerName;
    public string Description => powerDescription;
    public int powerInvested {get; set;}
    public virtual void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        powerInvested = source.Investment;
        source.ClearInvestment();
    }
}
