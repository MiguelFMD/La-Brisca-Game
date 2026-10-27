using UnityEngine;

public class TrickCleanUpState : TrickState
{
    public TrickCleanUpState(TrickManager trickManager) : base(trickManager) {}

    public override void Enter()
    {
        if (_trickManager.firstPlayer.currentHealth <= 0 || _trickManager.secondPlayer.currentHealth <= 0)
        {
            return; 
        }
        
        //Check if game over
        if(BriscaRules.CheckGameOverConditions(_trickManager.firstPlayer.state, _trickManager.secondPlayer.state))
        {
            _trickManager.RaiseGameOver();
        }
        else
        {
            _trickManager.firstPlayer.state.PlayedCard = null;
            _trickManager.secondPlayer.state.PlayedCard = null;
            _trickManager.DealOneCard();
            _trickManager.RaiseTrickEnded();
            _trickManager.firstPlayer = null;
            _trickManager.secondPlayer = null;
            _trickManager.ChangeState(new WaitingTurnState(_trickManager));
        }
       
    }

    public override void Exit() {}
}
