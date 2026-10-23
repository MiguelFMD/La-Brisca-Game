using System;
using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    public Hand playerHand;
    public Dictionary<CardData, CardDisplay> cardVisuals = new Dictionary<CardData, CardDisplay>();
    [Header("Listener events")]
    [SerializeField] private CardPayloadGameEvent onAddedCard;
    [SerializeField] private CardGameEvent onCardPlayed;
    [SerializeField] private PlayerGameEvent onClearHand;
    [Header("Raise events")]
    [SerializeField] private CardGameEvent onCardToTable;

    void OnValidate()
    {
        if(onAddedCard == null) Debug.LogError($"[Critical] '{name}' needs 'onAddedCard' in order to work.", this);
        if(onClearHand == null) Debug.LogError($"[Critical] '{name}' needs 'onClearHand' in order to work.", this);
        if(onCardPlayed == null) Debug.LogError($"[Critical] '{name}' needs 'onCardPlayed' in order to work.", this);
    }

    void OnEnable()
    {
        if(onAddedCard != null) onAddedCard.OnRaised += AddDisplayCard;
        if(onClearHand != null) onClearHand.OnRaised += RemoveDisplayedCards;
        if(onCardPlayed != null) onCardPlayed.OnRaised += HandleOnCardPlayed;
    }

    void OnDisable()
    {
        if(onAddedCard != null) onAddedCard.OnRaised -= AddDisplayCard;
        if(onClearHand != null) onClearHand.OnRaised -= RemoveDisplayedCards;
        if(onCardPlayed != null) onCardPlayed.OnRaised -= HandleOnCardPlayed;
    }

    public CardDisplay ExtractCardDisplay(CardData cardData)
    {
        if (cardVisuals.TryGetValue(cardData, out CardDisplay cardVisual))
        {
            cardVisuals.Remove(cardData);
            cardVisual.transform.SetParent(null);
            return cardVisual;
        }
        return null;
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

    private void HandleOnCardPlayed(CardData cardData)
    {
        if(cardData.owner != playerHand.playerOwner) return;
        onCardToTable.Raise(cardData);
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
