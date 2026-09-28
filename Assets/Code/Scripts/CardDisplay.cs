using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

public class CardDisplay : MonoBehaviour
{
    private IObjectPool<CardDisplay> pool;
    private Card cardData;
    [SerializeField] private Image image;
    private Player playerOwner;
    private Button cardButton;

    private void OnEnable()
    {
        cardButton.onClick.AddListener(() => OnButtonClick(this));
    }

    private void OnDisable()
    {
        cardButton.onClick.RemoveAllListeners();
    }

    public void SetCardData(Card newCardData, Player owner)
    {
        cardData = newCardData;
        image.sprite = cardData.GetCardSprite();
        playerOwner = owner;
    }

    public Card GetCardData()
    {
        return cardData;
    }

    public void OnButtonClick(CardDisplay cardDisplay)
    {
        EventManager.OnCardButtonClicked?.Invoke(cardDisplay);
    }

    public Player GetPlayerOwner()
    {
        return playerOwner;
    }

    /*public SetPool(IObjectPool<CardDisplay> newPool)
    {
        pool = newPool;
        CancelInvoke();
        Invoke(nameof(ReturntoPool));
    }*/



}
