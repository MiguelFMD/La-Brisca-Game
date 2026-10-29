using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private DeckType deckType;
    private Queue<CardStatus> deckCards;
    private PlayerState owner;

    public void AssignDeck(DeckType newDeck)
    {
        deckType = newDeck;
    }
    
    public CardStatus RemoveCard()
    {
        if(!IsDeckEmpty())
        {
            return deckCards.Dequeue();
        }
            
        else
        {
            Debug.LogWarning("No cards left in the deck");
            return null;
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
        deckCards = new Queue<CardStatus>();
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
