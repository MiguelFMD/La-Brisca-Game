using UnityEngine;

[RequireComponent(typeof(Hand))]
public class Player : MonoBehaviour
{
    // --- ESTADO LÓGICO PURO ---
    public PlayerState state { get; private set; }

    // --- PROPIEDADES PUENTE (No rompen las referencias de tus otros scripts) ---
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
    [SerializeField] private float defaultMaxHealth = 100f; // Añadido para inicializar el estado
    
    void Awake()
    {
        hand = GetComponent<Hand>();
        
        // 1. Inicializamos el estado lógico (C# puro)
        state = new PlayerState(defaultMaxHealth);

        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }

    public void Reset()
    {
        hand.ClearHand();
        state = new PlayerState(defaultMaxHealth); // Reseteamos la lógica también
        Heal(maxHealth);
        CreateDeck();
    }

    public void SetPlayerData(DeckType newDeck, float newMaxHealth)
    {
        deck.AssignDeck(newDeck);
        state.SetMaxHealth(newMaxHealth); // Se pasa al estado
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
        // 1. Modificamos la lógica pura
        state.PlayCard(newPlayedCard);
        
        // 2. Reflejamos en la interfaz visual (MonoBehaviour)
        hand.PlayCard(newPlayedCard);
    }

    public void PlayRandomCard()
    {
        if(hand.cards.Count > 0)
        {
            int randomC = UnityEngine.Random.Range(0, hand.cards.Count);
            PlayCard(hand.cards[randomC]);
        }
    }

    public void DrawOneCard()
    {
        CardData newCard = deck.RemoveCard();
        if(newCard == null) return;
        
        newCard.owner = this; 
        
        // 1. Registramos en la lógica pura
        state.Hand.Add(newCard);
        
        // 2. Mostramos en la mano visual
        hand.DrawCard(newCard);
    }

    public bool HasNoCardsLeft()
    {
        return IsHandEmpty() && IsDeckEmpty();
    }
    
    private bool IsHandEmpty()
    {
        // Leemos desde el estado lógico
        return state.Hand.Count == 0;
    }

    private bool IsDeckEmpty()
    {
        // Para sincronización, mantenemos la lectura del visual, pero podrías leer de state.Deck.Count
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
                // ¡Importante usar PlayCard aquí para que actualice la lógica y no solo lo visual!
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
        // 1. Modificamos la lógica pura
        state.TakeDamage(amount);
        
        // 2. Notificamos a los eventos de Unity
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
        
        if (state.IsDead) // Usamos la propiedad del estado lógico
        {
            print("dead");
            onPlayerDeath.Raise(this);
        }
    }

    public void Heal(float amount)
    {
        // 1. Modificamos la lógica pura
        state.Heal(amount);
        
        // 2. Notificamos a los eventos de Unity
        if(onPlayerHealthChanged != null) onPlayerHealthChanged.Raise(this);
    }
}