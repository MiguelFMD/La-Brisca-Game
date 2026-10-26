public abstract class TrickState
{
    protected TrickManager _trickManager;
    public TrickState(TrickManager trickManager)
    {
        _trickManager = trickManager;
    }
    public abstract void Enter();
    public abstract void Exit();

    public virtual void CardSelected(CardData cardData)
    {
        
    }

    public virtual void CardsDealt(Player player)
    {
        
    }

    public virtual void AnimationEnded()
    {
        
    }
}
