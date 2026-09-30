using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public List<Card> scoredCards;
    public Card playedCard;
    public int playerNumber;
    private Hand hand;

    //Events
    public event Action<float, float> OnPlayerHealthChanged;
    //public event Action<Player> OnPlayerDamaged;
    
    [Header("Player Stats")]
    public float maxHealth = 100.0f;
    public float currentHealth;
    public int maxEnergy = 3;
    public int currentEnergy;

    void OnEnable()
    {
        EventManager.OnCardButtonClicked += HandleButtonClicked;
    }

    void OnDisable()
    {
        EventManager.OnCardButtonClicked -= HandleButtonClicked;
    }

    void Awake()
    {
        hand = GetComponent<Hand>();
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /*public void PlayRandomCard()
    {
        if(hand.cards.Count == 0)
            Debug.LogError("Hand from player " + this.name + " is empty!");
        else
        {
            int random = Random.Range(0, hand.cards.Count - 1);
            PlayCard(hand.displayedCards.GetChild(random));
        }
    }*/

    //------CARDS FUNCTIONS----------
    public void PlayCard(CardDisplay newPlayedCard)
    {
        playedCard = newPlayedCard.GetCardData();
        hand.PlayCard(playedCard);
        //print("Card played is: " + playedCard);
    }

    public void DrawCard(Card newCard)
    {
        hand.DrawCard(newCard, this);
    }

    public void ClearHand()
    {
        hand.ClearHand();
    }

    public void ClearPlayedCard()
    {
        playedCard = null;
    }

    public int CalculateScore()
    {
        int score = 0;
        foreach(Card card in scoredCards)
        {
            score += card.CalculateCardValue();
        }
        return score;
    }

    public bool IsHandEmpty()
    {
        return hand.cards.Count == 0;
    }

    private void HandleButtonClicked(CardDisplay cardDisplay)
    {
        if(cardDisplay.GetPlayerOwner() == this)
            GameManager.Instance.PlayCard(cardDisplay);
    }

    //------STATS FUNCTIONS-------
    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
        {
            // Lógica de derrota/muerte
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void ChangeCurrentEnergy(int value)
    {
        currentEnergy = Math.Clamp(currentEnergy + value, 0, maxEnergy);
    }

}
