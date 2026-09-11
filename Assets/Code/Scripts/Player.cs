using UnityEngine;

public class Player : MonoBehaviour
{
    public int score;
    public Card playedCard;
    private Hand hand;

    void Awake()
    {
        hand = GetComponent<Hand>();
    }

    public void PlayRandomCard()
    {
        int random = Random.Range(1, hand.maxCardsAmount);
        playedCard = hand.cards[random];
        print("Card played is: " + playedCard);
        hand.PlayCard(playedCard);
    }

    public void DrawCard(Card newCard)
    {
        hand.DrawCard(newCard);
    }

    public void ClearHand()
    {
        hand.ClearHand();
    }

    public void ClearPlayedCard()
    {
        playedCard = null;
    }

    public void SetPlayerScore()
    {
        
    }
}
