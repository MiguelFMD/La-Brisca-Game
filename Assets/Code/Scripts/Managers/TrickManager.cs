using UnityEngine;
using UnityEngine.UI;

public class TrickManager : MonoBehaviour
{
    [Header("Table references")]
    [SerializeField] private Image triumphCardDisplay;
    [SerializeField] private CardData[] triumphCards;
    [Header("Listeners Events")]
    [SerializeField] private GameEvent onGameLoaded;
    [SerializeField] private CardGameEvent onCardSelected;
    //[SerializeField] private PlayerGameEvent onCardsDealt;
    [Header("Raise Events")]
    [SerializeField] private GameEvent onTrickEnded;
    [SerializeField] private GameEvent onCardAnimationEnded;
    [SerializeField] private GameEvent onGameOver;
    [SerializeField] private PlayerGameEvent onPlayerVictory;

    private TrickState currentState;
    [SerializeField] private Player[] players = new Player[2];
    [Header("Debug")]
    [SerializeField] private string currentStateName = "Ninguno";
    public Player[] Players => players;
    public Player firstPlayer {get; set;}
    public Player secondPlayer {get; set;}
    public Suit triumphSuit {get; set;}
    public int currentPlayer {get; set;}

    //-----CORE FUNCTIONS------
    void OnValidate()
    {
        if(onGameLoaded == null) Debug.LogError($"[Critical] '{name}' needs 'onGameLoaded' in order to work.", this);
        if(onCardSelected == null) Debug.LogError($"[Critical] '{name}' needs 'onCardSelected' in order to work.", this);
        //if(onCardsDealt == null) Debug.LogError($"[Critical] '{name}' needs 'onCardsDealt' in order to work.", this);
        if(onGameOver == null) Debug.LogError($"[Critical] '{name}' needs 'onGameOver' in order to work.", this);
        if(onCardAnimationEnded == null) Debug.LogError ($"[Critical] '{name}' needs 'onCardAnimationEnded' in order to work.", this);
    }
    void OnEnable()
    {
        if(onGameLoaded != null) onGameLoaded.OnRaised += ResetTrick;
        if(onCardSelected != null) onCardSelected.OnRaised += CardSelected;
        //if(onCardsDealt != null) onCardsDealt.OnRaised += HandleCardsDealt;
        if(onCardAnimationEnded != null) onCardAnimationEnded.OnRaised += HandleAnimationEnded;
    }
    void OnDisable()
    {
        if(onGameLoaded != null) onGameLoaded.OnRaised -= ResetTrick;
        if(onCardSelected != null) onCardSelected.OnRaised -= CardSelected;
        //if(onCardsDealt != null) onCardsDealt.OnRaised -= HandleCardsDealt;
        if(onCardAnimationEnded != null) onCardAnimationEnded.OnRaised -= HandleAnimationEnded;
    }

    private void ResetTrick()
    {
        foreach(Player player in players)
        {
            player.Reset();
        }
        currentPlayer = Random.Range(0, players.Length);
        TrickDealingState trickDealingState = new TrickDealingState(this);
        ChangeState(trickDealingState);
    }

    public void ChangeState(TrickState newState)
    {
        string oldStateName = currentState != null ? currentState.GetType().Name : "Ninguno";
        if(currentState != null) currentState.Exit();
        currentState = newState;
        Debug.Log($"<color=cyan>[FSM]</color> {oldStateName} ---> {currentState.GetType().Name}");
        currentStateName = currentState.GetType().Name;
        currentState.Enter();
    }

    private void CardSelected(CardData cardData)
    {
        currentState?.CardSelected(cardData);
    }

    public bool CheckPlayerTurn(Player owner)
    {
        if(players[currentPlayer] != owner)
        {
            Debug.LogWarning("Wait for your turn!");
            return false;
        }
        else
            return true;
    }

    public void SelectNextPlayer()
    {
        if(currentPlayer >= players.Length - 1)
            currentPlayer = 0;
        else
            currentPlayer++;
    }

    public bool CheckAllPlayersHavePlayed()
    {
        foreach(Player player in players)
        {
            if(player.playedCard == null)
                return false;
        }
        return true;
    }

    public void HandleAnimationEnded()
    {
        currentState?.AnimationEnded();
    }

    public void RaiseTrickEnded()
    {
        if(onTrickEnded != null) onTrickEnded.Raise();
    }

    public void RaiseGameOver()
    {
        if(onGameOver != null) onGameOver.Raise();
    }

    public void RaisePlayerWin(Player player)
    {
        if(onPlayerVictory != null) onPlayerVictory.Raise(player);
    }

    public void DealInitalCards()
    {
        foreach(Player player in players)
        {
            player.DrawCards(player.initialCardsAmount);
        }
    }

    public void DealOneCard()
    {
        foreach(Player player in players)
        {
            player.DrawOneCard();
        }
    }
    
    public void DiscoverTriumphSuit()
    {
        Suit newTriumphSuit = (Suit)Random.Range(0, System.Enum.GetValues(typeof(Suit)).Length);
        foreach(CardData card in triumphCards)
        {
            if(card.GetCardSuit() == newTriumphSuit)
            {
                triumphCardDisplay.sprite = card.GetCardSprite();
                break; 
            }
        }
        triumphCardDisplay.gameObject.SetActive(true);
        triumphSuit = newTriumphSuit;
    }
}
