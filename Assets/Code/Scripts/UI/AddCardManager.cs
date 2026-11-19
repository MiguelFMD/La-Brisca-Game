using UnityEngine;
using UnityEngine.UI;

public class AddCardManager : MonoBehaviour
{
    [SerializeField] private DeckType possibleCards;
    [SerializeField] private Transform visualCards;

    public void SetCardsData()
    {
        int randomIndex;
        for(int i = 0; i < visualCards.childCount; i++)
        {
            randomIndex = Random.Range(0, possibleCards.cards.Length);
            visualCards.GetChild(i).GetComponent<Image>().sprite = possibleCards.cards[randomIndex].GetCardSprite();
        }
    }
}
