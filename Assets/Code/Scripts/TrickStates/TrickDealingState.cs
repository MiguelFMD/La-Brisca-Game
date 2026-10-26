using UnityEngine;

public class TrickDealingState : TrickState
{
    private int playersPrepared;
    public TrickDealingState(TrickManager trickManager) : base(trickManager)
    {
    }

    public override void Enter()
    {
        playersPrepared = 0;
        _trickManager.DiscoverTriumphSuit();
    }

    public override void Exit()
    {
        
    }

    public override void CardsDealt(Player player)
    {
        playersPrepared++;
        if(playersPrepared == _trickManager.Players.Length)
        {
            //_trickManager.ChangeState(new WaitingTurnState(_trickManager));
        }
    }
}
