using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private DeckType deckType;
    private Queue<CardData> deckCards;

    public void AssignDeck(DeckType newDeck)
    {
        deckType = newDeck;
    }
    
    public CardData RemoveCard()
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
        deckCards = new Queue<CardData>();
        InitialShuffle();
    }
    private void InitialShuffle(int? seed = null)
    {
        deckCards.Clear();
        if (deckType == null || deckType.cards == null || deckType.cards.Length == 0)
            return;
        CardData[] cards = (CardData[])deckType.cards.Clone();

        System.Random prng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();

        for (int i = cards.Length - 1; i > 0; i--)
        {
            int randomIndex = prng.Next(0, i + 1);
            CardData temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            deckCards.Enqueue(cards[i]);
        }
    }
}
