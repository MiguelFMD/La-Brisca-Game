using UnityEngine;

public class TrickDealingState : TrickState
{
    public TrickDealingState(TrickManager trickManager) : base(trickManager) {}

    public override void Enter()
    {
        _trickManager.DiscoverTriumphSuit();
        _trickManager.DealInitalCards();
        _trickManager.ChangeState(new WaitingTurnState(_trickManager));
    }

    public override void Exit() {}
}
