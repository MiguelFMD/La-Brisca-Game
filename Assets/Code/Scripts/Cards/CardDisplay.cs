using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(CardPower))]
public class CardDisplay : MonoBehaviour
{
    private Button cardButton;
    private CardStatus cardStatus;
    private CardPower cardPower;
    [SerializeField] private CardGameEvent onCardSelected;
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

    public void SetCardData(CardStatus newCardStatus)
    {
        cardStatus = newCardStatus;
        image.sprite = cardStatus.cardData.GetCardSprite();
        if(newCardStatus.owner.IsEnemy)
        {
            cardButton.interactable = false;
        }
        else
        {
            cardButton.interactable = true;
        }
        //cardPower.SetCardPowerData(newCardData);
    }

    public CardData GetCardData()
    {
        return cardStatus.cardData;
    }

    public PlayerState GetPlayerOwner()
    {
        return cardStatus.owner;
    }

    /*public void UseMyPower(Player trickLoser)
    {
        cardPower.InvokePower(cardStatus.cardData.owner, trickLoser);
    }*/

    private void OnButtonClick()
    {
        onCardSelected.Raise(cardStatus);
    }
}
