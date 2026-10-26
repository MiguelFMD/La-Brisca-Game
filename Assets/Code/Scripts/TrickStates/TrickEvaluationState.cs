using UnityEngine;

public class TrickEvaluationState: TrickState
{
    public TrickEvaluationState(TrickManager trickManager) : base(trickManager) {}

    public override void Enter()
    {
        TrickResult trickResult = BriscaRules.CalculateTrickWinner(_trickManager.triumphSuit, _trickManager.firstPlayer, _trickManager.secondPlayer);
        if(trickResult != null)
        {
            trickResult.loser.TakeDamage(BriscaRules.CalculateCardValue(trickResult.winner.playedCard));
            //activate powers
            for(int i = 0; i < _trickManager.Players.Length; i++) //Next player to play is the winner
            {
                if(_trickManager.Players[i] == trickResult.winner)
                {
                    _trickManager.currentPlayer = i;
                    break;
                }
            }
        }
        else //Draw
        {
            Debug.LogWarning("DRAW!");
        }
        _trickManager.ChangeState(new TrickCleanUpState(_trickManager));
    }

    public override void Exit() {}
}
