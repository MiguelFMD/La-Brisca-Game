using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(CardPower))]
public class CardDisplay : MonoBehaviour
{
    [SerializeField] private CardGameEvent onCardSelected;
    [SerializeField] private CardGameEvent onReturnCard;
    private Button cardButton;
    private Card cardData;
    private CardPower cardPower;
    [SerializeField] private Image image;
    private Player playerOwner;

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

    public void SetCardData(Card newCardData, Player owner)
    {
        cardData = newCardData;
        image.sprite = cardData.GetCardSprite();
        playerOwner = owner;
        if(playerOwner.isEnemy)
        {
            cardButton.interactable = false;
        }
        else
        {
            cardButton.interactable = true;
        }
        cardPower.SetCardPowerData(newCardData);
    }

    public Card GetCardData()
    {
        return cardData;
    }

    public Player GetPlayerOwner()
    {
        return playerOwner;
    }

    public void UseMyPower(Player trickLoser)
    {
        cardPower.InvokePower(playerOwner, trickLoser);
    }

    public void ReturnToPool()
    {
        if(onReturnCard != null) onReturnCard.Raise(this);
    }

    private void OnButtonClick()
    {
        onCardSelected.Raise(this);
    }
}
