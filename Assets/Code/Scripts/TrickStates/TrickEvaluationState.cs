using UnityEngine;

public class TrickEvaluationState: TrickState
{
    public TrickEvaluationState(TrickManager trickManager) : base(trickManager) {}

    public override void Enter()
    {
        TrickResult trickResult = BriscaRules.CalculateTrickWinner(_trickManager.triumphSuit, _trickManager.firstPlayer.state, _trickManager.secondPlayer.state);
        if(trickResult != null)
        {
            Player loser = _trickManager.firstPlayer.state == trickResult.loser 
            ? _trickManager.firstPlayer 
            : _trickManager.secondPlayer;
            Player winner = _trickManager.firstPlayer.state == trickResult.winner 
            ? _trickManager.firstPlayer 
            : _trickManager.secondPlayer;

            
            //Normal damage 
            int damageAmount = BriscaRules.CalculateCardValue(trickResult.winner.PlayedCard.cardData);
            
            loser.TakeDamage(damageAmount);
            for(int i = 0; i < _trickManager.players.Length; i++) //Next player to play is the winner
            {
                if(_trickManager.players[i].state == trickResult.winner)
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
