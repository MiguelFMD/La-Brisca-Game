using UnityEngine;

[RequireComponent(typeof(Hand))]
public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] [Range(1, 5)] public int initialCardsAmount = 3;
    [SerializeField] public bool isEnemy = false;
    private Hand hand;
    [SerializeField] private DeckType playerDeck;
    
    [Header("Raise Events")]
    [SerializeField] private PlayerGameEvent onPlayerHealthChanged;
    [SerializeField] private PlayerGameEvent onPlayerDeath;
    [SerializeField] private PlayerGameEvent onDeckCardRemoved;
    [SerializeField] private PlayerGameEvent onStatusEffectChanged;
    [SerializeField] private PlayerGameEvent onInvestmentChanged;
    [SerializeField] private PlayerGameEvent onShieldChanged;
   
    private float defaultMaxHealth = 100f;
    public PlayerState state { get; private set; }
    public CardStatus playedCard => state?.PlayedCard;
    

    void Awake()
    {
        hand = GetComponent<Hand>();
        state = new PlayerState(defaultMaxHealth);
        state.IsEnemy = isEnemy;
        state.onCardPlayed += PlayCard;
        state.onPlayerDeath += RaisePlayerDeath;
        state.onPlayerHealthChanged += RaisePlayerHealthChanged;
        state.onDeckCardRemoved += RaiseOnDeckCardRemoved;
        state.onInvestmentChanged += RaiseOnInvestChanged;
        state.onShieldChanged += RaiseOnShieldChanged;
        state.onStatusEffectChanged += RaiseOnStatusEffectChanged;
        state.Deck = new Deck();
    }

    void OnDisable()
    {
        state.onCardPlayed -= PlayCard;
        state.onPlayerDeath -= RaisePlayerDeath;
        state.onPlayerHealthChanged -= RaisePlayerHealthChanged;
        state.onDeckCardRemoved -= RaiseOnDeckCardRemoved;
        state.onInvestmentChanged -= RaiseOnInvestChanged;
        state.onShieldChanged -= RaiseOnShieldChanged;
        state.onStatusEffectChanged -= RaiseOnStatusEffectChanged;
    }

    public void Reset()
    {
        hand.ClearHand();
        if(!state.IsEnemy) SetPlayerData(playerDeck, defaultMaxHealth);
        ResetStats();
        CreateDeck();
    }

    public void SetPlayerData(DeckType newDeck, float newMaxHealth)
    {
        state.Deck.AssignDeck(newDeck);
        state.SetMaxHealth(newMaxHealth);
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }

    //------CARDS FUNCTIONS----------
    private void CreateDeck()
    {
        state.Deck.CreateDeck(this);
    }

    private void PlayCard(CardStatus newPlayedCard)
    {
        hand.PlayCard(newPlayedCard);
    }

    public CardStatus GetRandomCard()
    {
        int randomC = Random.Range(0, hand.cards.Count);
        return hand.cards[randomC];
    }

    public void DrawCards(int amount)
    {
        for(int c = 0; c < amount; c++)
        {
            DrawOneCard();
        }
    }

    public void DrawOneCard()
    {
        CardStatus newCard = state.DrawOneCard();
        if(newCard == null) return;
        hand.DrawCard(newCard);
    }

    //------STATS FUNCTIONS-------
    public void TakeDamage(DamageContext ctx)
    {
        state.TakeDamage(ctx);
        
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
        
        if (state.IsDead) onPlayerDeath.Raise(this);
    }

    private void RaisePlayerHealthChanged()
    {
        onPlayerHealthChanged.Raise(this);
    }

    private void RaisePlayerDeath()
    {
        onPlayerDeath.Raise(this);
    }

    public void Heal(float amount)
    {
        state.Heal(amount);
        
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }

    private void ResetStats()
    {
        Heal(defaultMaxHealth);
        state.ResetShield();
        state.ClearInvestment();
        state.ClearStatus();
    }

    private void RaiseOnDeckCardRemoved()
    {
        onDeckCardRemoved.Raise(this);
    }

    private void RaiseOnInvestChanged()
    {
        onInvestmentChanged.Raise(this);
    }

    private void RaiseOnStatusEffectChanged()
    {
        onStatusEffectChanged.Raise(this);
    }

    private void RaiseOnShieldChanged()
    {
        onShieldChanged.Raise(this);
    }
}
