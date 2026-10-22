using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct CardPayload
{
    public CardData card;
    public Player owner;
    public CardPayload(CardData newCard, Player newOwner)
    {
        card = newCard;
        owner = newOwner;
    }
}

public class Hand : MonoBehaviour
{
    public List<CardData> cards;
    public int maxCardsAmount;
    public Player playerOwner;
    
    [Header("Events")]
    [SerializeField] private CardPayloadGameEvent onAddedCard;
    [SerializeField] private PlayerGameEvent onClearHand;
    [SerializeField] private CardGameEvent onCardPlayed;


    void Awake()
    {
        cards = new List<CardData>();
        playerOwner = GetComponent<Player>();
    }

    public CardData PlayCard(CardData playedCard)
    {
        //print("Played card is: " + selectedCard.GetCardRank() + " of " + selectedCard.GetCardSuit());
        if(RemoveCard(playedCard))
        {
            onCardPlayed.Raise(playedCard);
            return playedCard;
        }
            
        else
            return null;
    }

    public void DrawCard(CardData newCard, Player owner)
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
    private bool AddCard(CardData newCard, Player owner)
    {
        if(cards.Count >= maxCardsAmount)
            return false;
        
        cards.Add(newCard);
        //AddDisplayCard(newCard, owner);
        CardPayload cardPayload = new CardPayload(newCard, owner);
        onAddedCard.Raise(cardPayload);
        return true;
    }
    
    private bool RemoveCard(CardData selectedCard)
    {
        return cards.Remove(selectedCard);
    }
}
