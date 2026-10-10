using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    public List<Card> cards;
    public int maxCardsAmount;
    public RectTransform displayedCards;

    void Awake()
    {
        cards = new List<Card>();
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
        print("c");
        AddCard(newCard, owner);
    }

    public void ClearHand()
    {
        cards.Clear();
        RemoveDisplayedCards();
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
        AddDisplayCard(newCard, owner);
        return true;
    }

    private void AddDisplayCard(Card newCardData, Player owner)
    {
        CardDisplay newCardDisplay = GameManager.Instance.GetCardVisual();
        newCardDisplay.transform.SetParent(displayedCards, false);
        newCardDisplay.transform.localScale = Vector3.one;
        newCardDisplay.SetCardData(newCardData, owner);
    }

    ///--------------------Removing cards----------------

    /// <summary>
    /// Tryes to remove a card from the hand and returns true if succesfully removed the card, false if not.
    /// </summary>
    private bool RemoveCard(Card selectedCard)
    {
        return cards.Remove(selectedCard);
    }

    private void RemoveDisplayedCards()
    {
        for(int c = displayedCards.childCount-1; c >= 0; c--)
        {
            CardDisplay card = displayedCards.GetChild(c).GetComponent<CardDisplay>();
            GameManager.Instance.ReturnCardToPool(card);
        }
    }


    /*public GameObject FindCardByCardData(Card selectedCard)
    {
        for(int c = 0; c < displayedCards.childCount; c++)
        {
            if(displayedCards.GetChild(c).GetComponent<CardDisplay>().GetCardData() == selectedCard)
            {
                return displayedCards.GetChild(c).gameObject;
            }
        }
        return null;
    }*/
}
