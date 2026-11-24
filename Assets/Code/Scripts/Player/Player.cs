using UnityEngine;

[RequireComponent(typeof(Hand))]
public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] [Range(1, 5)] public int initialCardsAmount = 3;
    [SerializeField] public bool isEnemy = false;
    [SerializeField] private float defaultMaxHealth = 100f;
    private Hand hand;
    [SerializeField] private DeckType playerDeck;
    
    [Header("Raise Events")]
    [SerializeField] private PlayerGameEvent onPlayerHealthChanged;
    [SerializeField] private PlayerGameEvent onPlayerDeath;
    [SerializeField] private PlayerGameEvent onDeckCardRemoved;
    [SerializeField] private PlayerGameEvent onStatusEffectChanged;
    [SerializeField] private PlayerGameEvent onInvestmentChanged;
    [SerializeField] private PlayerGameEvent onShieldChanged;
   
    public PlayerState state { get; private set; }
    public CardStatus playedCard => state?.PlayedCard;
    

    void Awake()
    {
        hand = GetComponent<Hand>();
        state = new PlayerState(defaultMaxHealth)
        {
            IsEnemy = isEnemy
        };
        state.onCardPlayed += PlayCard;
        state.onPlayerDeath += RaisePlayerDeath;
        state.onPlayerHealthChanged += RaisePlayerHealthChanged;
        state.onDeckCardRemoved += RaiseOnDeckCardRemoved;
        state.onInvestmentChanged += RaiseOnInvestChanged;
        state.onShieldChanged += RaiseOnShieldChanged;
        state.onStatusEffectChanged += RaiseOnStatusEffectChanged;
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
        if (hand == null) hand = GetComponent<Hand>();
        if (hand != null && hand.cards != null) hand.ClearHand();
        if (state == null) return;

        if (!state.IsEnemy && playerDeck != null) SetPlayerData(playerDeck, defaultMaxHealth);
        ResetStats();
        CreateDeck();
    }

    private void ResetStats()
    {
        state.Heal(defaultMaxHealth);
        state.ResetShield();
        state.ClearInvestment();
        state.ClearStatus();
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

    public void OnAddedCard(CardData cardData)
    {
        CardStatus cardStatus = new CardStatus(cardData, state);
        state.Deck.AddCard(cardStatus);
    }

    //Events


    private void RaisePlayerHealthChanged()
    {
        Debug.Log($"<color=orange>[Player]</color> {this.name} health: {state.CurrentHealth} / {state.MaxHealth}");
        onPlayerHealthChanged.Raise(this);
    }

    private void RaisePlayerDeath()
    {
        onPlayerDeath.Raise(this);
    }

    private void RaiseOnDeckCardRemoved()
    {
        Debug.Log($"<color=blue>[Player]</color> {this.name} cards: {state.Deck.CountCards()}");
        onDeckCardRemoved.Raise(this);
    }

    private void RaiseOnInvestChanged()
    {
        Debug.Log($"<color=yellow>[Player]</color> {this.name} invest: {state.Investment}");
        onInvestmentChanged.Raise(this);
    }

    private void RaiseOnStatusEffectChanged()
    {
        Debug.Log($"<color=purple>[Player]</color> {this.name} added status");
        onStatusEffectChanged.Raise(this);
    }

    private void RaiseOnShieldChanged()
    {
        Debug.Log($"<color=green>[Player]</color> {this.name} shield: {state.CurrentShield}");
        onShieldChanged.Raise(this);
    }
}
