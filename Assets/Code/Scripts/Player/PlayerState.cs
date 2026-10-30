using System;
using System.Collections.Generic;

public class PlayerState
{
     public event Action<CardStatus> onCardPlayed;
    public float MaxHealth {get; private set;}
    public float CurrentHealth {get; private set;}
    public bool IsDead => CurrentHealth <= 0;
    public List<CardStatus> Hand { get; } = new List<CardStatus>();
    public Queue<CardStatus> Deck { get; } = new Queue<CardStatus>();
    public CardStatus PlayedCard { get; set; }
    public bool IsEnemy {get; set; }

    public PlayerState(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        MaxHealth = newMaxHealth;
        CurrentHealth = newMaxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Math.Max(0, CurrentHealth - amount);
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
    }

    public void LoadDeck(IEnumerable<CardStatus> cards)
    {
        Deck.Clear();
        foreach (var card in cards)
        {
            Deck.Enqueue(card);
        }
    }

    public CardStatus DrawCard()
    {
        if (Deck.Count == 0) return null;

        CardStatus card = Deck.Dequeue();
        Hand.Add(card);
        return card;
    }

    public bool PlayCard(CardStatus card)
    {
        if (!Hand.Contains(card)) return false;

        Hand.Remove(card);
        PlayedCard = card;
        onCardPlayed?.Invoke(card);
        return true;
    }

    public void ClearPlayedCard()
    {
        PlayedCard = null;
    }

    public bool HasNoCardsLeft() => Hand.Count == 0 && Deck.Count == 0;
}
