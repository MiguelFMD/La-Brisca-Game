using System.Collections.Generic;
using UnityEngine;

public class CardPoolManager : MonoBehaviour
{
    [Header("Pool references")]
    [SerializeField] private GameObject cardDisplayPrefab;
    [SerializeField] private RectTransform cardPoolTransform;
    [SerializeField] private CardPoolSO cardPoolService;
    [Header("Events")]
    [SerializeField] private CardGameEvent onReturnCard;

    private Queue<CardDisplay> cardPool = new Queue<CardDisplay>();
    private Dictionary<CardData, CardDisplay> cardDisplays = new Dictionary<CardData,CardDisplay>();

    void OnEnable()
    {
        if(onReturnCard != null) onReturnCard.OnRaised += ReturnCardToPool;
        cardPoolService.RequestCard = ProvideCard;
    }

    void OnDisable()
    {
        if(onReturnCard != null) onReturnCard.OnRaised -= ReturnCardToPool;
    }

    private CardDisplay ProvideCard()
    {
        if (cardPool.Count > 0)
        {
            CardDisplay pooledCard = cardPool.Dequeue();
            pooledCard.gameObject.SetActive(true);
            cardDisplays.Remove(pooledCard.GetCardData());
            return pooledCard;
        }
        else
        {
            GameObject newCard = Instantiate(cardDisplayPrefab);
            CardDisplay newCardDisplay = newCard.GetComponent<CardDisplay>();
            return newCardDisplay;
        }
    }

    private void ReturnCardToPool(CardData cardToReturn)
    {
        if (cardDisplays.TryGetValue(cardToReturn, out CardDisplay cardDisplay))
        {
            cardDisplay.gameObject.SetActive(false); // Hide the card
            cardPool.Enqueue(cardDisplay); // Save it for future
            cardDisplays.Add(cardDisplay.GetCardData(), cardDisplay);
            cardDisplay.transform.SetParent(cardPoolTransform);
        }
    }
}
