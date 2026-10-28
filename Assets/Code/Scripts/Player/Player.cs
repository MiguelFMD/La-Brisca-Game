using UnityEngine;

[RequireComponent(typeof(Hand))]
public class Player : MonoBehaviour
{
    public PlayerState state { get; private set; }

    public CardData playedCard => state?.PlayedCard;
    public float maxHealth => state != null ? state.MaxHealth : 100f;
    public float currentHealth => state != null ? state.CurrentHealth : maxHealth;
    [SerializeField] public bool isEnemy = false;
    private Hand hand;
    [SerializeField] private Deck deck;
    
    [Header("Raise Events")]
    [SerializeField] private PlayerGameEvent onPlayerHealthChanged;
    [SerializeField] private PlayerGameEvent onPlayerDeath;
   
    [Header("Player Stats")]
    [SerializeField] public int initialCardsAmount = 3;
    [SerializeField] private float defaultMaxHealth = 100f; 
    void Awake()
    {
        hand = GetComponent<Hand>();
        state = new PlayerState(defaultMaxHealth);
    }

    public void Reset()
    {
        hand.ClearHand();
        //state = new PlayerState(defaultMaxHealth);
        Heal(maxHealth);
        CreateDeck();
    }

    public void SetPlayerData(DeckType newDeck, float newMaxHealth)
    {
        deck.AssignDeck(newDeck);
        state.SetMaxHealth(newMaxHealth);
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }

    //------CARDS FUNCTIONS----------

    public void DrawCards(int amount)
    {
        for(int c = 0; c < amount; c++)
        {
            DrawOneCard();
        }
    }

    private void CreateDeck()
    {
        deck.CreateDeck();
    }

    public void PlayCard(CardData newPlayedCard)
    {
        state.PlayCard(newPlayedCard);
        
        hand.PlayCard(newPlayedCard);
    }

    public void PlayRandomCard()
    {
        if(hand.cards.Count > 0)
        {
            int randomC = Random.Range(0, hand.cards.Count);
            PlayCard(hand.cards[randomC]);
        }
    }

    public CardData GetRandomCard()
    {
        int randomC = Random.Range(0, hand.cards.Count);
        return hand.cards[randomC];
    }

    public void DrawOneCard()
    {
        CardData newCard = deck.RemoveCard();
        if(newCard == null) return;
        
        newCard.owner = this; 
        
        state.Hand.Add(newCard);
        
        hand.DrawCard(newCard);
    }

    public bool HasNoCardsLeft()
    {
        return IsHandEmpty() && IsDeckEmpty();
    }
    
    private bool IsHandEmpty()
    {
        return state.Hand.Count == 0;
    }

    private bool IsDeckEmpty()
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
                PlayCard(card);
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
        state.TakeDamage(amount);
        
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
        
        if (state.IsDead)
        {
            onPlayerDeath.Raise(this);
        }
    }

    public void Heal(float amount)
    {
        state.Heal(amount);
        
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }
}