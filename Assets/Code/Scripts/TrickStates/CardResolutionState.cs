using UnityEngine;

public class CardResolutionState: TrickState
{
    private CardData _cardToPlay;
    public CardResolutionState(TrickManager trickManager, CardData cardToPlay) : base(trickManager)
    {
        _cardToPlay = cardToPlay;
    }

    public override void Enter()
    {
        _cardToPlay.owner.PlayCard(_cardToPlay);
    }

    public override void Exit() {}

    public override void AnimationEnded()
    {
        _trickManager.SelectNextPlayer();
        if(_trickManager.CheckAllPlayersHavePlayed())
        {
            _trickManager.ChangeState(new TrickEvaluationState(_trickManager));
        }
        else
        {
            _trickManager.ChangeState(new WaitingTurnState(_trickManager));
        }
    }

}
