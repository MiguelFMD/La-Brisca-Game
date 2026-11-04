using UnityEngine;

[RequireComponent(typeof(Hand))]
public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] [Range(1, 5)] public int initialCardsAmount = 3;
    [SerializeField] public bool isEnemy = false;
    private Hand hand;
    
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
    }

    public void Reset()
    {
        hand.ClearHand();
        Heal(maxHealth);
        CreateDeck();
    }

    public void SetPlayerData(DeckType newDeck, float newMaxHealth)
    {
        state.Deck.AssignDeck(newDeck);
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

    public void DrawOneCard()
    {
        CardStatus newCard = state.Deck.RemoveCard();
        if(newCard == null) return;
        
        newCard.owner = state; 
        
        state.Hand.Add(newCard);
        
        hand.DrawCard(newCard);
    }

    //------STATS FUNCTIONS-------
    public void TakeDamage(float amount)
    {
        state.TakeDamage(amount);
        
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
        
        if (state.IsDead) onPlayerDeath.Raise(this);
    }

    public void Heal(float amount)
    {
        state.Heal(amount);
        
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }
}