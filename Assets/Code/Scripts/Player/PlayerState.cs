using System;
using System.Collections.Generic;

public class PlayerState
{
    public float MaxHealth {get; private set;}
    public float CurrentHealth {get; private set;}
    public bool IsDead => CurrentHealth <= 0;
    public List<CardData> Hand { get; } = new List<CardData>();
    public Queue<CardData> Deck { get; } = new Queue<CardData>();
    public CardData PlayedCard { get; private set; }

    public PlayerState(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        MaxHealth = newMaxHealth;
        CurrentHealth = Math.Min(CurrentHealth, MaxHealth);
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

    public void LoadDeck(IEnumerable<CardData> cards)
    {
        Deck.Clear();
        foreach (var card in cards)
        {
            Deck.Enqueue(card);
        }
    }

    public CardData DrawCard()
    {
        if (Deck.Count == 0) return null;

        CardData card = Deck.Dequeue();
        Hand.Add(card);
        return card;
    }

    public bool PlayCard(CardData card)
    {
        if (!Hand.Contains(card)) return false;

        Hand.Remove(card);
        PlayedCard = card;
        return true;
    }

    public void ClearPlayedCard()
    {
        PlayedCard = null;
    }

    public bool HasNoCardsLeft() => Hand.Count == 0 && Deck.Count == 0;
}
