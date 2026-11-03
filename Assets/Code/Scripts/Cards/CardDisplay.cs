using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CardDisplay : MonoBehaviour
{
    private Button cardButton;
    private CardStatus cardStatus;
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
    }

    public CardData GetCardData()
    {
        return cardStatus.cardData;
    }

    public PlayerState GetPlayerOwner()
    {
        return cardStatus.owner;
    }

    private void OnButtonClick()
    {
        onCardSelected.Raise(cardStatus);
    }
}
