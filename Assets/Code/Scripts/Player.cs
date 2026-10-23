using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hand))]
public class Player : MonoBehaviour
{
    public List<CardData> scoredCards;
    public CardData playedCard;
    public bool isEnemy = false;
    private Hand hand;
    [SerializeField] private Deck deck;
    [Header("Listener Events")]
    [SerializeField] private GameEvent onGameLoaded;
    [SerializeField] private GameEvent onTrickEnded;
    [Header("Raise Events")]
    [SerializeField] private PlayerGameEvent onCardsDealt;
    [SerializeField] private PlayerGameEvent onPlayerHealthChanged;
    [SerializeField] private PlayerGameEvent onPlayerDeath;
   
    
    [Header("Player Stats")]
    [SerializeField] private int initialCardsAmount = 3;
    public float maxHealth = 100.0f;
    public float currentHealth;

    void OnValidate()
    {
        if(onGameLoaded == null) Debug.LogError($"[Critical] '{name}' needs 'onGameLoaded' in order to work.", this);
        if(onTrickEnded == null) Debug.LogError($"[Critical] '{name}' needs 'onTrickEnded' in order to work.", this);
        if(onCardsDealt == null) Debug.LogError($"[Critical] '{name}' needs 'onCardsDealt' in order to work.", this);
    }

    void OnEnable()
    {
        if(onGameLoaded != null) onGameLoaded.OnRaised += Reset;
        if(onTrickEnded != null) onTrickEnded.OnRaised += HandleOnTrickEnded;
    }

    void OnDisable()
    {
        if(onGameLoaded != null) onGameLoaded.OnRaised -= Reset;
        if(onTrickEnded != null) onTrickEnded.OnRaised -= HandleOnTrickEnded;
    }
    
    void Awake()
    {
        hand = GetComponent<Hand>();
        currentHealth = maxHealth;
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }

    private void Reset()
    {
        hand.ClearHand();
        scoredCards.Clear();
        Heal(maxHealth);
        CreateDeck();
        DrawCards(initialCardsAmount);
        onCardsDealt.Raise(this);
    }

    public void SetPlayerData(DeckType newDeck, float newMaxHealth)
    {
        deck.AssignDeck(newDeck);
        maxHealth = newMaxHealth;
    }

    //------CARDS FUNCTIONS----------

    private void DrawCards(int amount)
    {
        for(int c = 0; c < amount; c++)
        {
            DrawOneCard();
        }
    }

    private void HandleOnTrickEnded()
    {
        playedCard = null;
        DrawOneCard();
    }

    private void CreateDeck()
    {
        deck.CreateDeck();
    }

    public void PlayCard(CardData newPlayedCard)
    {
        playedCard = newPlayedCard;
        print("Card played is: " + playedCard);
        hand.PlayCard(playedCard);
        
    }

    public void PlayRandomCard()
    {
        int randomC = UnityEngine.Random.Range(0, hand.cards.Count);
        print("Play random card");
        PlayCard(hand.cards[randomC]);
        //return hand.cards[randomC];
    }

    public void DrawOneCard()
    {
        CardData newCard = deck.RemoveCard();
        newCard.owner = this;
        if(newCard != null)
            hand.DrawCard(newCard);
    }

    public int CalculateScore()
    {
        int score = 0;
        foreach(CardData card in scoredCards)
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

    public void TriggerPlayerDeath()
    {
        onPlayerDeath.Raise(this);
    }
    
    //Powers related

    public void UseCardPower(Player trickLoser)
    {
        //playedCard.UseMyPower(trickLoser);
    }

    public void RemoveRandomCard() //For clubs power
    {
        if(!IsHandEmpty())
        {
            int random = UnityEngine.Random.Range(0, hand.cards.Count);
            CardData card = hand.cards[random];
            if(card != null)
            {
                hand.PlayCard(card);
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
        //print("Take damage: " + currentHealth);
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
        if (currentHealth <= 0)
        {
            print("dead");
            onPlayerDeath.Raise(this);
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
        //OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
