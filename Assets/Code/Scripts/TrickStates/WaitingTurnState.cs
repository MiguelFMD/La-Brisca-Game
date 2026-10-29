using UnityEngine;

public class WaitingTurnState : TrickState
{
    public WaitingTurnState(TrickManager trickManager) : base(trickManager) {}

    public override void Enter()
    {
        if(_trickManager.players[_trickManager.currentPlayer].isEnemy)
        {
            CardStatus enemyCard = _trickManager.players[_trickManager.currentPlayer].GetRandomCard();
            //_trickManager.Players[_trickManager.currentPlayer].PlayRandomCard();
            if(_trickManager.firstPlayer == null) _trickManager.firstPlayer = _trickManager.players[_trickManager.currentPlayer];
            else _trickManager.secondPlayer = _trickManager.players[_trickManager.currentPlayer];
            _trickManager.ChangeState(new CardResolutionState(_trickManager, enemyCard));
        }
    }

    public override void Exit(){}

    public override void CardSelected(CardStatus cardStatus)
    {
        if(_trickManager.CheckPlayerTurn(cardStatus.owner))
        {
            if(_trickManager.firstPlayer == null) _trickManager.firstPlayer = _trickManager.players[_trickManager.currentPlayer];
            else _trickManager.secondPlayer = _trickManager.players[_trickManager.currentPlayer];
            _trickManager.ChangeState(new CardResolutionState(_trickManager, cardStatus));
        }
    }
}
