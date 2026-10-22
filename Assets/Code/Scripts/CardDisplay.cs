using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(CardPower))]
public class CardDisplay : MonoBehaviour
{
    [SerializeField] private CardGameEvent onCardSelected;
    [SerializeField] private CardGameEvent onReturnCard;
    private Button cardButton;
    private CardData cardData;
    private CardPower cardPower;
    [SerializeField] private Image image;

    void OnEnable()
    {
        cardButton.onClick.AddListener(OnButtonClick);
    }

    void OnDisable()
    {
        cardButton.onClick.RemoveListener(OnButtonClick);
    }

    void Awake()
    {
        cardButton = GetComponent<Button>();
        cardPower = GetComponent<CardPower>();
    }

    public void SetCardData(CardData newCardData)
    {
        cardData = newCardData;
        image.sprite = cardData.GetCardSprite();
        if(newCardData.owner.isEnemy)
        {
            cardButton.interactable = false;
        }
        else
        {
            cardButton.interactable = true;
        }
        cardPower.SetCardPowerData(newCardData);
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public Player GetPlayerOwner()
    {
        return cardData.owner;
    }

    public void UseMyPower(Player trickLoser)
    {
        cardPower.InvokePower(cardData.owner, trickLoser);
    }

    public void ReturnToPool()
    {
        if(onReturnCard != null) onReturnCard.Raise(cardData);
    }

    private void OnButtonClick()
    {
        onCardSelected.Raise(cardData);
    }
}
