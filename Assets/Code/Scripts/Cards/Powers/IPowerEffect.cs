public interface IPowerEffect
{
    string Name {get;}
    string Description {get;}
    void Activate(CardData cardData, Player source, Player target);
}
