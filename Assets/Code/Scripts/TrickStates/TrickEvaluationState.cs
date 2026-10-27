using UnityEngine;

public class TrickEvaluationState: TrickState
{
    public TrickEvaluationState(TrickManager trickManager) : base(trickManager) {}

    public override void Enter()
    {
        TrickResult trickResult = BriscaRules.CalculateTrickWinner(_trickManager.triumphSuit, _trickManager.firstPlayer.state, _trickManager.secondPlayer.state);
        if(trickResult != null)
        {
            Player loserMonoBehaviour = _trickManager.firstPlayer.state == trickResult.loser 
            ? _trickManager.firstPlayer 
            : _trickManager.secondPlayer;

            // 2. Llamamos al TakeDamage del MonoBehaviour (que actualizará el state y disparará el evento visual)
            int damageAmount = BriscaRules.CalculateCardValue(trickResult.winner.PlayedCard);
            loserMonoBehaviour.TakeDamage(damageAmount);
            //activate powers
            for(int i = 0; i < _trickManager.Players.Length; i++) //Next player to play is the winner
            {
                if(_trickManager.Players[i].state == trickResult.winner)
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
