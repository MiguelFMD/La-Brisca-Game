using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<CardData> cards;
    public int maxCardsAmount;
    public Player playerOwner;
    
    [Header("Events")]
    [SerializeField] private CardGameEvent onAddedCard;
    [SerializeField] private PlayerGameEvent onClearHand;
    [SerializeField] private CardGameEvent onCardPlayed;


    void Awake()
    {
        cards = new List<CardData>();
        playerOwner = GetComponent<Player>();
    }

    public CardData PlayCard(CardData selectedCard)
    {
        CardData playedCard = selectedCard;
        //print("Played card is: " + selectedCard.GetCardRank() + " of " + selectedCard.GetCardSuit());
        if(RemoveCard(playedCard))
        {
            onCardPlayed.Raise(playedCard);
            return playedCard;
        }
            
        else
            return null;
    }

    public void DrawCard(CardData newCard)
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
    private bool AddCard(CardData newCard)
    {
        if(cards.Count >= maxCardsAmount)
            return false;
        
        cards.Add(newCard);
        onAddedCard.Raise(newCard);
        return true;
    }
    
    private bool RemoveCard(CardData selectedCard)
    {
        return cards.Remove(selectedCard);
    }
}
