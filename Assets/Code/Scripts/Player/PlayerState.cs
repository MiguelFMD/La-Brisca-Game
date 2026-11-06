using System;
using System.Collections.Generic;

public class PlayerState
{
    public Deck Deck {get; set; }
    public event Action<CardStatus> onCardPlayed;
    public float MaxHealth {get; private set;}
    public float CurrentHealth {get; private set;}
    public float CurrentShield {get; set;}
    public bool IsDead => CurrentHealth <= 0;
    public List<CardStatus> Hand { get; } = new List<CardStatus>();
    public List<StatusEffect> StatusEffects {get;} = new List<StatusEffect>();
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

    public void TakeDamage(DamageContext ctx)
    {
        if (IsDead) return;
        if(CurrentShield > 0)
        {
            float amountLeft = CurrentShield - ctx.Amount;
            CurrentHealth = Math.Max(0, CurrentHealth - amountLeft);
        } 
        else CurrentHealth = Math.Max(0, CurrentHealth - ctx.Amount);
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
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

    public bool HasNoCardsLeft() => Hand.Count == 0 && Deck.CountCards() == 0;

    //Status Effects
    public void ClearStatusEffects()
    {
        StatusEffects.Clear();
    }

    public void AddStatusEffect(StatusEffect newStatusEffect)
    {
        StatusEffects.Add(newStatusEffect);
    }
}
