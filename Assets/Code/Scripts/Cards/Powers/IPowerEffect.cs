public interface IPowerEffect
{
    string Name {get;}
    string Description {get;}
    void Activate(CardData cardData, PlayerState source, PlayerState target);
}
