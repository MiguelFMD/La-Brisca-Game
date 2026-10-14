using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(CardPower))]
public class CardDisplay : MonoBehaviour
{
    [SerializeField] private CardGameEvent onCardSelected;
    private Button cardButton;
    private Card cardData;
    private CardPower cardPower;
    [SerializeField] private Image image;
    private Player playerOwner;

    void Awake()
    {
        cardButton = GetComponent<Button>();
        cardPower = GetComponent<CardPower>();
        cardButton.onClick.AddListener(OnButtonClick);
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

    public void OnButtonClick()
    {
        onCardSelected.Raise(this);
    }

    public Player GetPlayerOwner()
    {
        return playerOwner;
    }

    public void UseMyPower(Player trickLoser)
    {
        cardPower.InvokePower(playerOwner, trickLoser);
    }
}
