using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct CardPayload
{
    public Card card;
    public Player owner;
    public CardPayload(Card newCard, Player newOwner)
    {
        card = newCard;
        owner = newOwner;
    }
}

public class Hand : MonoBehaviour
{
    public List<Card> cards;
    public int maxCardsAmount;
    public Player playerOwner;
    
    [Header("Events")]
    public CardPayloadGameEvent onAddedCard;
    public PlayerGameEvent onClearHand;

    void Awake()
    {
        cards = new List<Card>();
        playerOwner = GetComponent<Player>();
    }

    public Card PlayCard(Card selectedCard)
    {
        //print("Played card is: " + selectedCard.GetCardRank() + " of " + selectedCard.GetCardSuit());
        if(RemoveCard(selectedCard))
            return selectedCard;
        else
            return null;
    }

    public void DrawCard(Card newCard, Player owner)
    {
        AddCard(newCard, owner);
    }

    public void ClearHand()
    {
        cards.Clear();
        onClearHand.Raise(playerOwner);
        //RemoveDisplayedCards();
    }

    ///--------------------Adding cards--------------------

    /// <summary>
    /// Adds a card.
    /// </summary>
    private bool AddCard(Card newCard, Player owner)
    {
        if(cards.Count >= maxCardsAmount)
            return false;
        
        cards.Add(newCard);
        //AddDisplayCard(newCard, owner);
        CardPayload cardPayload = new CardPayload(newCard, owner);
        onAddedCard.Raise(cardPayload);
        return true;
    }
    
    private bool RemoveCard(Card selectedCard)
    {
        return cards.Remove(selectedCard);
    }
}
