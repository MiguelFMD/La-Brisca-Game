using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    [SerializeField] private Hand playerHand;

    void OnEnable()
    {
        playerHand.onAddedCard.OnRaised += AddDisplayCard;
        playerHand.onClearHand.OnRaised += RemoveDisplayedCards;
    }

    void OnDisable()
    {
        playerHand.onAddedCard.OnRaised -= AddDisplayCard;
        playerHand.onClearHand.OnRaised -= RemoveDisplayedCards;
    }

    private void AddDisplayCard(CardPayload cardPayload)
    {
        if(cardPayload.owner != playerHand.playerOwner) return;
        CardDisplay newCardDisplay = GameManager.Instance.GetCardVisual();
        newCardDisplay.transform.SetParent(transform, false);
        newCardDisplay.transform.localScale = Vector3.one;
        print(cardPayload.owner);
        newCardDisplay.SetCardData(cardPayload.card, cardPayload.owner);
    }

    private void RemoveDisplayedCards(Player owner)
    {
        if(playerHand.playerOwner != owner) return;
        for(int c = transform.childCount-1; c >= 0; c--)
        {
            CardDisplay card = transform.GetChild(c).GetComponent<CardDisplay>();
            card.ReturnToPool();
        }
    }
}
