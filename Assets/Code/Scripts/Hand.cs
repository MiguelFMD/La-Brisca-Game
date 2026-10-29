using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField][Range(1, 10)] private int maxCardsAmount = 3;
    [Header("Debug")]
    public List<CardStatus> cards;
    public Player playerOwner;
    
    [Header("Events")]
    [SerializeField] private CardGameEvent onAddedCard;
    [SerializeField] private PlayerGameEvent onClearHand;
    [SerializeField] private CardGameEvent onCardPlayed;


    void Awake()
    {
        cards = new List<CardStatus>();
        playerOwner = GetComponent<Player>();
    }

    public CardStatus PlayCard(CardStatus selectedCardStatus)
    {
        if(RemoveCard(selectedCardStatus))
        {
            onCardPlayed.Raise(selectedCardStatus);
            return selectedCardStatus;
        }
            
        else
            return null;
    }

    public void DrawCard(CardStatus newCard)
    {
        AddCard(newCard);
    }

    public void ClearHand()
    {
        cards.Clear();
        onClearHand.Raise(playerOwner);
    }

    ///--------------------Adding cards--------------------

    /// <summary>
    /// Adds a card.
    /// </summary>
    private bool AddCard(CardStatus newCardStatus)
    {
        if(cards.Count >= maxCardsAmount)
            return false;
        
        cards.Add(newCardStatus);
        onAddedCard.Raise(newCardStatus);
        return true;
    }
    
    private bool RemoveCard(CardStatus selectedCard)
    {
        return cards.Remove(selectedCard);
    }
}
