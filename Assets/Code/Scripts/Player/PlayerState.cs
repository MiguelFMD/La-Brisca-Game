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
    public List<StatusEffect> activeStatusEffects = new List<StatusEffect>();
    public CardStatus PlayedCard { get; set; }
    public bool IsEnemy {get; set; }
    public int Investment {get; private set;} //Applied to card played value

    public PlayerState(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Investment = 0;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        MaxHealth = newMaxHealth;
        CurrentHealth = newMaxHealth;
    }

    public void TakeDamage(DamageContext ctx)
    {
        if (IsDead) return;
        DamageContext newContext = ctx;
        float amountLeft = ctx.Amount;
        if(CurrentShield > 0) amountLeft -= CurrentShield; 
        if(amountLeft > 0) 
        {
            CurrentHealth = Math.Max(0, CurrentHealth - amountLeft);
            newContext.Amount = amountLeft;
        }
        foreach(StatusEffect statusEffect in activeStatusEffects)
        {
            statusEffect.OnAfterReceiveDamage(newContext);
        }
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

    public CardStatus DrawOneCard()
    {
        CardStatus newCard = Deck.RemoveCard();
        if(newCard == null) return newCard;
        newCard.owner = this; 
        Hand.Add(newCard);
        foreach(StatusEffect statusEffect in activeStatusEffects)
        {
            statusEffect.OnAfterDrawnCard(this);
        }
        return newCard;
    }

    public void ClearPlayedCard()
    {
        PlayedCard = null;
    }

    public bool HasNoCardsLeft() => Hand.Count == 0 && Deck.CountCards() == 0;

    //Status Effects
    public void AddStatusEffect(StatusEffect newStatusEffect)
    {
        foreach(StatusEffect statusEffect in activeStatusEffects)
        {
            if(statusEffect.GetType() == newStatusEffect.GetType())
            {
                statusEffect.AddStacks(newStatusEffect.Stacks);
                return;
            }
        }
    }

    public void ClearStatus()
    {
        activeStatusEffects.Clear();
    }

    public void AddInvestment(int amount)
    {
        Investment += amount;
    }


}
