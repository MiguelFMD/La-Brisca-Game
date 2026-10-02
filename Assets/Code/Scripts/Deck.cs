using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private DeckType deckType;
    //[SerializeField] private SpriteRenderer triumphCardDisplay;
    //private Card triumphSuitCard;
    private Queue<Card> deckCards;
    
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
            /*if(triumphSuitCard != null)
            {
                Debug.LogWarning("No cards left, giving the triumph card");
                //triumphCardDisplay.gameObject.SetActive(false);
                return triumphSuitCard;
            }
            else
            {
                
                return null;
            }*/
            
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

    /*public Card DiscoverTriumphSuit()
    {
        print("Discovering triumph suit...");
        triumphSuitCard = RemoveCard();
        triumphCardDisplay.sprite = triumphSuitCard.GetCardSprite();
        triumphCardDisplay.gameObject.SetActive(true);
        print("The triumph suit is: " + triumphSuitCard.GetCardSuit());
        return triumphSuitCard;
    }*/

    private void InitialShuffle()
    {
        //print("Shuffe deck");
        deckCards.Clear();
        List<Card> cards = new List<Card>();
        foreach(Card card in deckType.cards)
        {
            cards.Add(card);
        }

        int random = 0;

        for(int c = 0; c < deckType.cards.Length - 1; c++)
        {
            random = Random.Range(0, cards.Count);
            Card card = cards[random];
            cards.RemoveAt(random);
            deckCards.Enqueue(card);
        }
        //Card card = cards[random];
        deckCards.Enqueue(cards[0]);
        //print(deckCards);
    }

    
}
