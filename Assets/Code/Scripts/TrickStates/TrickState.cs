public abstract class TrickState
{
    protected TrickManager _trickManager;
    public TrickState(TrickManager trickManager)
    {
        _trickManager = trickManager;
    }
    public abstract void Enter();
    public abstract void Exit();

    public virtual void CardSelected(CardStatus cardStatus){}

    public virtual void AnimationEnded(){}
}
