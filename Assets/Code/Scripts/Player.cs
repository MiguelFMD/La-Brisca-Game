using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hand))]
public class Player : MonoBehaviour
{
    public List<Card> scoredCards;
    public Card playedCard;
    public CardDisplay playedCardDisplay;
    //public int playerNumber;
    private Hand hand;
    [SerializeField] private Deck deck;
    public bool isEnemy = false;

    [Header("Events")]
    [SerializeField] private PlayerGameEvent onPlayerHealthChanged;
    public event Action OnPlayerDead;
    
    [Header("Player Stats")]
    public float maxHealth = 100.0f;
    public float currentHealth;
    public int maxEnergy = 3;
    public int currentEnergy;

    void Awake()
    {
        hand = GetComponent<Hand>();
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }

    void Start()
    {
        Reset();
    }

    public void SetPlayerData(DeckType newDeck, float newMaxHealth)
    {
        deck.AssignDeck(newDeck);
        maxHealth = newMaxHealth;
    }

    private void Reset()
    {
        ClearHand();
        scoredCards.Clear();
        Heal(maxHealth);
        CreateDeck();
    }
    
    //------CARDS FUNCTIONS----------
    public void PlayCard(CardDisplay newPlayedCard)
    {
        print(isEnemy);
        playedCardDisplay = newPlayedCard;
        playedCard = newPlayedCard.GetCardData();
        hand.PlayCard(playedCard);
        //print("Card played is: " + playedCard);
    }

    public CardDisplay PlayRandomCard()
    {
        int randomC = UnityEngine.Random.Range(0, hand.cards.Count);
        CardDisplay randomCard = hand.displayedCards.GetChild(randomC).GetComponent<CardDisplay>();
        PlayCard(randomCard);
        return randomCard;
    }

    public void DrawCard()
    {
        Card newCard = deck.RemoveCard();
        if(newCard != null)
            hand.DrawCard(newCard, this);
    }

    public void CreateDeck()
    {
        deck.CreateDeck();
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

    public bool IsDeckEmpty()
    {
        return deck.IsDeckEmpty();
    }

    public void TriggerPlayerDead()
    {
        OnPlayerDead?.Invoke();
    }
    
    //Powers related

    public void UseCardPower(Player trickLoser)
    {
        playedCardDisplay.UseMyPower(trickLoser);
    }

    public void RemoveRandomCard() //For clubs power
    {
        if(!IsHandEmpty())
        {
            int random = UnityEngine.Random.Range(0, hand.cards.Count);
        
            Card card = hand.cards[random];
            if(card != null)
            {
                hand.PlayCard(card);
                GameManager.Instance.ReturnCardToPool(hand.displayedCards.GetChild(random).GetComponent<CardDisplay>());
            }
        }
        else
        {
            Debug.LogWarning("Player's hand is empty");
        }
    }

    //------STATS FUNCTIONS-------
    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        onPlayerHealthChanged.Raise(this);
        //OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnPlayerDead?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onPlayerHealthChanged.Raise(this);
        //OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void ChangeCurrentEnergy(int value)
    {
        currentEnergy = Math.Clamp(currentEnergy + value, 0, maxEnergy);
    }

}
