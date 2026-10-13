using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private DeckType deckType;
    private Queue<Card> deckCards;

    public void AssignDeck(DeckType newDeck)
    {
        deckType = newDeck;
    }
    
    public Card RemoveCard()
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
    public void CreateDeck()
    {
        //print("Creating deck...");
        deckCards = new Queue<Card>();
        InitialShuffle();
    }
    private void InitialShuffle()
    {
        deckCards.Clear();
        List<Card> cards = new List<Card>();
        foreach(Card card in deckType.cards)
        {
            cards.Add(card);
        }

        int random;

        for(int c = 0; c < deckType.cards.Length - 1; c++)
        {
            random = Random.Range(0, cards.Count);
            Card card = cards[random];
            cards.RemoveAt(random);
            deckCards.Enqueue(card);
        }
        deckCards.Enqueue(cards[0]);
    }
}
