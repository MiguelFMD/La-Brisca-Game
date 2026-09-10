using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] Card[] initialCards;
    private Queue<Card> deckCards;
    private Card.Suit triumphSuit;

    void Start()
    {
        CreateDeck();
    }

    private void CreateDeck()
    {
        foreach(Card card in initialCards)
        {
            deckCards.Enqueue(card);
        }
    }
}
