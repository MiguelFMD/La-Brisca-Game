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
   
    private float defaultMaxHealth = 100f;
    public PlayerState state { get; private set; }
    public CardStatus playedCard => state?.PlayedCard;
    public float maxHealth => state != null ? state.MaxHealth : 100f;
    public float currentHealth => state != null ? state.CurrentHealth : maxHealth;
    

    void Awake()
    {
        hand = GetComponent<Hand>();
        state = new PlayerState(defaultMaxHealth);
        state.IsEnemy = isEnemy;
        state.onCardPlayed += PlayCard;
        state.Deck = new Deck();
    }

    public void Reset()
    {
        hand.ClearHand();
        if(!state.IsEnemy) SetPlayerData(playerDeck, maxHealth);
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

    public void Heal(float amount)
    {
        state.Heal(amount);
        
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }

    private void ResetStats()
    {
        Heal(maxHealth);
        state.CurrentShield = 0;
        state.ClearStatus();
    }
}