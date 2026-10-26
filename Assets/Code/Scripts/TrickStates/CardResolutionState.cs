using UnityEngine;

public class CardResolutionState: TrickState
{
    public CardResolutionState(TrickManager trickManager) : base(trickManager) {}

    public override void Enter()
    {
        Debug.Log("Card resolution state");
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
