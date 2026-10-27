using System.Collections.Generic;
using UnityEngine;

public class CardPoolManager : MonoBehaviour
{
    [Header("Pool references")]
    [SerializeField] private GameObject cardDisplayPrefab;
    [SerializeField] private CardPoolSO cardPoolService;
    private Queue<CardDisplay> cardPool = new Queue<CardDisplay>();
    private RectTransform cardPoolTransform;

    void Awake()
    {
        cardPoolTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        cardPoolService.RequestCard = ProvideCard;
        cardPoolService.ReturnCard = ReturnCardToPool;
    }

    private CardDisplay ProvideCard()
    {
        if (cardPool.Count > 0)
        {
            CardDisplay pooledCard = cardPool.Dequeue();
            pooledCard.gameObject.SetActive(true);
            return pooledCard;
        }
        else
        {
            GameObject newCard = Instantiate(cardDisplayPrefab);
            CardDisplay newCardDisplay = newCard.GetComponent<CardDisplay>();
            return newCardDisplay;
        }
    }

    private void ReturnCardToPool(CardDisplay cardToReturn)
    {
        cardToReturn.gameObject.SetActive(false); // Hide the card
        cardPool.Enqueue(cardToReturn); // Save it for future
        cardToReturn.transform.SetParent(cardPoolTransform);
    }
}
