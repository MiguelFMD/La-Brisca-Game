using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardVisualManager : MonoBehaviour
{
    [Header("Card animation")]
    [SerializeField] private float cardAnimationDuration;
    [SerializeField] private AnimationCurve easingCurve;
    [Header("Game references")]
    [SerializeField] private RectTransform tableRectTransform;
    [SerializeField] private HandView playerHandView;
    [SerializeField] private HandView enemyHandView;
    [Header("Raise Events")]
    [SerializeField] private GameEvent onCardAnimationEnded;
    [Header("Listener Events")]
    [SerializeField] private GameEvent onTrickEnded;
    [SerializeField] private CardGameEvent onCardToTable;

    [SerializeField] private CardPoolSO cardPoolSO;

    private List<CardDisplay> tableCards;

    void OnValidate()
    {
        if(onCardToTable == null) Debug.LogError($"[Critical] '{name}' needs 'onCardToTable' in order to work.", this);
        if(onTrickEnded == null) Debug.LogError($"[Critical] '{name}' needs 'onTrickEnded' in order to work.", this);
    }

    void OnEnable()
    {
        if(onCardToTable != null) onCardToTable.OnRaised += HandleOnCardToTable;
        if(onTrickEnded != null) onTrickEnded.OnRaised += HandleOnTrickEnded;
    }

    void OnDisable()
    {
        if(onCardToTable != null) onCardToTable.OnRaised -= HandleOnCardToTable;
        if(onTrickEnded != null) onTrickEnded.OnRaised -= HandleOnTrickEnded;
    }

    void Awake()
    {
        tableCards = new List<CardDisplay>();
    }

    private void HandleOnCardToTable(CardData cardData)
    {
        //print(cardData);
        CardDisplay cardDisplay;
        if(cardData.owner == playerHandView.playerHand.playerOwner)
            cardDisplay = playerHandView.ExtractCardDisplay(cardData);
        else if(cardData.owner == enemyHandView.playerHand.playerOwner)
            cardDisplay = enemyHandView.ExtractCardDisplay(cardData);
        else
        {
            Debug.LogError("No owner");
            return;
        }
        //print(cardDisplay);
        tableCards.Add(cardDisplay);
        CardToCenter(cardDisplay);
    }

    private void HandleOnTrickEnded()
    {
        ClearTableVisuals();
    }

    private void CardToCenter(CardDisplay cardDisplay)
    {
        RectTransform cardRect = cardDisplay.GetComponent<RectTransform>();
        cardRect.SetParent(tableRectTransform);
        StartCoroutine(AnimateCard(cardRect));
    }

    private IEnumerator AnimateCard(RectTransform selectedCard)
    {
        Vector2 targetPosition = tableRectTransform.anchoredPosition;
        Vector2 startPosition = selectedCard.anchoredPosition;
        float timeElapsed = 0f;
        while (timeElapsed < cardAnimationDuration)
        {
            timeElapsed += Time.deltaTime;
            float percentage = timeElapsed / cardAnimationDuration;
            
            // Evaluate the animation curve for smooth acceleration/deceleration
            float curveValue = easingCurve.Evaluate(percentage);
            
            selectedCard.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);
            yield return null; // Wait for the next frame
        }
        selectedCard.SetParent(tableRectTransform, true); // Snap to new parent
        onCardAnimationEnded.Raise();
    }

    private void ClearTableVisuals()
    {
        foreach(CardDisplay card in tableCards)
        {
            cardPoolSO.ReturnCard(card);
        }
        tableCards.Clear();
    }
}
