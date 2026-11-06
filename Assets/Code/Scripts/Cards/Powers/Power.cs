
public abstract class Power: IPowerEffect
{
    public string powerName {get; set;}
    public string powerDescription {get; set;}
    public string Name => powerName;
    public string Description => powerDescription;
    public virtual void Activate(CardData cardData, PlayerState source, PlayerState target)
    {
        
    }
}
