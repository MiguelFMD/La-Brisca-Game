using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    private List<Card> cards;
    private int maxCardsAmount;

    public void PlayCard(Card selectedCard)
    {
        RemoveCard(selectedCard);
    }


    /// <summary>
    /// Makes the player jump if they have remaining stamina.
    /// </summary>
    public void DrawCard(Card newCard)
    {
        AddCard(newCard);
    }

    /// <summary>
    /// Adds a card.
    /// </summary>
    private bool AddCard(Card newCard)
    {
        if(cards.Count >= maxCardsAmount)
            return false;
        
        cards.Add(newCard);
        return true;
    }

    /// <summary>
    /// Tryes to remove a card from the hand and returns true if succesfully removed the card, false if not.
    /// </summary>
    private bool RemoveCard(Card selectedCard)
    {
        return cards.Remove(selectedCard);
    }
}
