using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    [SerializeField] private Hand playerHand;
    private Dictionary<CardData, CardDisplay> cardVisuals = new Dictionary<CardData, CardDisplay>();
    
    [SerializeField] private CardPayloadGameEvent onAddedCard;
    [SerializeField] private CardGameEvent onCardPlayed;
    [SerializeField] private PlayerGameEvent onClearHand;

    void OnEnable()
    {
        onAddedCard.OnRaised += AddDisplayCard;
        onClearHand.OnRaised += RemoveDisplayedCards;
        onCardPlayed.OnRaised += HandleonCardPlayed;
    }

    void OnDisable()
    {
        onAddedCard.OnRaised -= AddDisplayCard;
        onClearHand.OnRaised -= RemoveDisplayedCards;
        onCardPlayed.OnRaised += HandleonCardPlayed;
    }

    private void AddDisplayCard(CardPayload cardPayload)
    {
        if(cardPayload.owner != playerHand.playerOwner) return;
        CardDisplay newCardDisplay = GameManager.Instance.GetCardVisual();
        newCardDisplay.transform.SetParent(transform, false);
        newCardDisplay.transform.localScale = Vector3.one;
        newCardDisplay.SetCardData(cardPayload.card);
        cardVisuals.Add(cardPayload.card, newCardDisplay);
    }

    private void HandleonCardPlayed(CardData cardData)
    {
        if(cardData.owner != playerHand.playerOwner) return;
        if(cardVisuals.TryGetValue(cardData, out CardDisplay cardDisplay))
        {
            Transform cardTranform = cardDisplay.transform;
            cardVisuals.Remove(cardData);
        }
    }

    private void RemoveDisplayedCards(Player owner)
    {
        if(playerHand.playerOwner != owner) return;
        foreach(var card in cardVisuals)
        {
            card.Value.ReturnToPool();
        }
        cardVisuals.Clear();
    }
    
}
