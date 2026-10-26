using UnityEngine;

public class WaitingTurnState : TrickState
{
    public WaitingTurnState(TrickManager trickManager) : base(trickManager) {}

    public override void Enter()
    {
        if(_trickManager.Players[_trickManager.currentPlayer].isEnemy)
        {
            _trickManager.Players[_trickManager.currentPlayer].PlayRandomCard();
            if(_trickManager.firstPlayer == null) _trickManager.firstPlayer = _trickManager.Players[_trickManager.currentPlayer];
            else _trickManager.secondPlayer = _trickManager.Players[_trickManager.currentPlayer];
            //_trickManager.ChangeState(new CardResolutionState(_trickManager));
        }
    }

    public override void Exit()
    {
        
    }

    public override void CardSelected(CardData cardData)
    {
        if(_trickManager.CheckPlayerTurn(cardData.owner))
        {
            cardData.owner.PlayCard(cardData);
            if(_trickManager.firstPlayer == null) _trickManager.firstPlayer = cardData.owner;
            else _trickManager.secondPlayer = cardData.owner;
            //_trickManager.ChangeState(new CardResolutionState(_trickManager));
        }
    }
}
