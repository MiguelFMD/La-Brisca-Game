using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using System;

[RequireComponent(typeof(Button))]
public class CardDisplay : MonoBehaviour
{
    private Button cardButton;
    private IObjectPool<CardDisplay> pool;
    private Card cardData;
    [SerializeField] private Image image;
    private Player playerOwner;

    void Awake()
    {
        cardButton = GetComponent<Button>();
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
    }

    public Card GetCardData()
    {
        return cardData;
    }

    public void OnButtonClick()
    {
        EventManager.OnCardButtonClicked?.Invoke(this);
    }

    public Player GetPlayerOwner()
    {
        return playerOwner;
    }
}
