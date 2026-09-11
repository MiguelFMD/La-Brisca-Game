using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Card[] initialCards;
    private Queue<Card> deckCards;

    void Awake()
    {
        CreateDeck();
    }

    public Card RemoveCard()
    {
        if(deckCards.Count > 0)
            return deckCards.Dequeue();
        else
            return null;
    }

    private void InitialShuffle()
    {
        print("Shuffe deck");
        deckCards.Clear();
        List<Card> cards = new List<Card>();
        foreach(Card card in initialCards)
        {
            cards.Add(card);
        }

        int random = 0;

        for(int c = 0; c < initialCards.Length - 1; c++)
        {
            random = Random.Range(1, cards.Count - 1);
            Card card = cards[random];
            cards.RemoveAt(random);
            //print(card.number);
            deckCards.Enqueue(card);
        }
        //Card card = cards[random];
        deckCards.Enqueue(cards[0]);
        print(deckCards);
    }

    private void CreateDeck()
    {
        print("Creating deck...");
        deckCards = new Queue<Card>();
        InitialShuffle();
    }
}
