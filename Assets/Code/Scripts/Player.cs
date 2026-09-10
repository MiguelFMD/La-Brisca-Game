using UnityEngine;

public class Player : MonoBehaviour
{
    public Hand hand;
    public int score;
    public Card playedCard;

    public void PlayRandomCard()
    {
        int random = Random.Range(1, hand.maxCardsAmount);
        playedCard = hand.cards[random];
        hand.PlayCard(playedCard);
    }

}
