using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private DeckType deckType;
    private Queue<CardStatus> deckCards = new Queue<CardStatus>();
    private Stack<CardStatus> deadCards = new Stack<CardStatus>();
    private PlayerState owner;
    public event Action onCardRemoved;

    public void AssignDeck(DeckType newDeck)
    {
        deckType = newDeck;
    }
    
    public CardStatus RemoveCard()
    {
        if(!IsDeckEmpty())
        {
            CardStatus removedCard = deckCards.Dequeue();
            deadCards.Push(removedCard);
            onCardRemoved?.Invoke();
            return removedCard;
        }
            
        else
        {
            Debug.LogWarning("No cards left in the deck");
            return null;
        }
            
    }

    public int CountCards()
    {
        return deckCards.Count;
    }
    public void ReviveCards(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            CardStatus card = deadCards.Pop();
            if(card != null) 
            {
                deckCards.Enqueue(card);
                return;
            }
        }
    }

    public void BurnCards(int amount)
    {
        if(deckCards.Count <= 0) return;
        for(int i = 0; i < amount; i++)
        {
            RemoveCard();
        }   
    }

    public bool IsDeckEmpty()
    {
        return deckCards.Count == 0;
    }

    /// <summary>
    /// Creates a new initial deck. Use this to start a new game.
    /// </summary>
    public void CreateDeck(Player newOwner)
    {
        owner = newOwner.state;
        InitialShuffle();
    }
    private void InitialShuffle(int? seed = null)
    {
        deckCards.Clear();
        if (deckType == null || deckType.cards == null || deckType.cards.Length == 0)
            return;

        CardStatus[] cards = new CardStatus[deckType.cards.Length];
        
        for(int i = 0; i < deckType.cards.Length; i++)
        {
            CardStatus newCard = new CardStatus(deckType.cards[i], owner);
            cards[i] = newCard;
        }

        System.Random prng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();

        for (int i = cards.Length - 1; i > 0; i--)
        {
            int randomIndex = prng.Next(0, i + 1);
            CardStatus temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            deckCards.Enqueue(cards[i]);
        }
    }
}
