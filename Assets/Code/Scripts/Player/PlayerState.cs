using System;
using System.Collections.Generic;

public class PlayerState
{
    public Deck Deck {get; set; }
    public event Action<CardStatus> onCardPlayed;
    public event Action onPlayerDeath;
    public event Action onPlayerHealthChanged;
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
        float amountLeft = ctx.Amount;
        if(CurrentShield > 0) 
        {
            amountLeft -= CurrentShield; 
            CurrentShield -= ctx.Amount;
        }
        if(amountLeft > 0) 
        {
            CurrentHealth = Math.Max(0, CurrentHealth - amountLeft);
            onPlayerHealthChanged?.Invoke();
        }
        if(IsDead) onPlayerDeath?.Invoke();
        foreach(StatusEffect statusEffect in activeStatusEffects)
        {
            statusEffect.OnAfterReceiveDamage(ctx);
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
            if(statusEffect.Stacks == 0) activeStatusEffects.Remove(statusEffect);
            else statusEffect.OnAfterDrawnCard(this);
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
        if(activeStatusEffects.Contains(newStatusEffect))
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
        else
        {
            activeStatusEffects.Add(newStatusEffect);
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

    public void ClearInvestment()
    {
        Investment = 0;
    }


}
