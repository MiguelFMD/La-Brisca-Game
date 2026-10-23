using System.Collections;
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

    private void HandleOnCardToTable(CardData cardData)
    {
        if(cardData.owner != playerHandView.playerHand)
        {
            CardDisplay cardDisplay = playerHandView.ExtractCardDisplay(cardData);
            CardToCenter(cardDisplay);
        }
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
        foreach(CardDisplay card in tableRectTransform)
        {
            card.ReturnToPool();
        }
    }
}
