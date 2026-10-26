using UnityEditor.Experimental.GraphView;
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
    [SerializeField] private PlayerGameEvent onCardsDealt;
    [SerializeField] private GameEvent onGameOver;
    [Header("Raise Events")]
    [SerializeField] private GameEvent onTrickEnded;
    [SerializeField] private GameEvent onCardAnimationEnded;
    [SerializeField] private Player[] players = new Player[2];
    private int playersPrepared = 0;
    private Player firstPlayer;
    private Player secondPlayer;
    private Suit triumphSuit;
    private int currentPlayer;
    private bool isNewTrickPlay = true;
    private bool isGameEnded = false;
    private bool isAnimationPlaying = false;

    //-----CORE FUNCTIONS------
    void OnValidate()
    {
        if(onGameLoaded == null) Debug.LogError($"[Critical] '{name}' needs 'onGameLoaded' in order to work.", this);
        if(onCardSelected == null) Debug.LogError($"[Critical] '{name}' needs 'onCardSelected' in order to work.", this);
        if(onCardsDealt == null) Debug.LogError($"[Critical] '{name}' needs 'onCardsDealt' in order to work.", this);
        if(onGameOver == null) Debug.LogError($"[Critical] '{name}' needs 'onGameOver' in order to work.", this);
        if(onCardAnimationEnded == null) Debug.LogError ($"[Critical] '{name}' needs 'onCardAnimationEnded' in order to work.", this);
    }
    void OnEnable()
    {
        if(onGameLoaded != null) onGameLoaded.OnRaised += ResetTrick;
        if(onCardSelected != null) onCardSelected.OnRaised += CardSelected;
        if(onCardsDealt != null) onCardsDealt.OnRaised += HandleCardsDealt;
        if(onGameOver != null) onGameOver.OnRaised += HandleGameOver;
        if(onCardAnimationEnded != null) onCardAnimationEnded.OnRaised += HandleAnimationEnded;
    }
    void OnDisable()
    {
        if(onGameLoaded != null) onGameLoaded.OnRaised -= ResetTrick;
        if(onCardSelected != null) onCardSelected.OnRaised -= CardSelected;
        if(onCardsDealt != null) onCardsDealt.OnRaised -= HandleCardsDealt;
        if(onCardAnimationEnded != null) onCardAnimationEnded.OnRaised -= HandleAnimationEnded;
    }

    private void ResetTrick()
    {
        currentPlayer = 1;// Random.Range(0, players.Length);
        isNewTrickPlay = true;
        DiscoverTriumphSuit();
        isGameEnded = false;
    }

    private void CardSelected(CardData cardData)
    {
        if(CheckPlayerTurn(cardData.owner) && !isAnimationPlaying) 
        {
            cardData.owner.PlayCard(cardData);
            isAnimationPlaying = true;
            if(isNewTrickPlay)
            {
                firstPlayer = players[currentPlayer];
                isNewTrickPlay = false; 
            }
            else
                secondPlayer = players[currentPlayer];
        }
    }

    private void HandleGameOver()
    {
        isGameEnded = true;
        playersPrepared = 0;
    }

    private void HandleCardsDealt(Player player)
    {
        playersPrepared++;
        if(playersPrepared == players.Length)
            CheckEnemyTurn();
    }

    private void CheckEnemyTurn()
    {
        if(players[currentPlayer].isEnemy) //Is enemy turn?
        {
            isAnimationPlaying = true;
            players[currentPlayer].PlayRandomCard();
            if(isNewTrickPlay)
            {
                firstPlayer = players[currentPlayer];
                isNewTrickPlay = false;
            }
            else
                secondPlayer = players[currentPlayer];
        }
        else print("No");
    }

    //-----PRIVATE FUNCTIONS------
    private void AddScoredCards(Player trickWinner, Player[] players)
    {
        foreach(Player player in players)
        {
            trickWinner.scoredCards.Add(player.playedCard);
        }
    }

    private bool CheckPlayerTurn(Player owner)
    {
        if(players[currentPlayer] != owner)
        {
            Debug.LogWarning("Wait for your turn!");
            return false;
        }
        else
            return true;
    }

    private void SelectNextPlayer()
    {
        if(currentPlayer >= players.Length - 1)
            currentPlayer = 0;
        else
            currentPlayer++;
        print("Next player is: " + players[currentPlayer]);
    }

    private bool CheckAllPlayersHavePlayed()
    {
        foreach(Player player in players)
        {
            if(player.playedCard == null)
                return false;
        }
        print("All players have played");
        return true;
    }

    private void HandleAnimationEnded()
    {
        print("Animation ended");
        SelectNextPlayer();  
        isAnimationPlaying = false;
        if(CheckAllPlayersHavePlayed())
        {
            TrickResult newTrickResult = BriscaRules.CalculateTrickWinner(triumphSuit, firstPlayer, secondPlayer);
            if(newTrickResult != null)
            {
                Player trickWinner = newTrickResult.winner;
                Player trickLoser = newTrickResult.loser;
                //trickWinner.UseCardPower(trickLoser);
                trickLoser.TakeDamage(BriscaRules.CalculateCardValue(trickWinner.playedCard));
                print("Ganador: " + trickWinner + " con " + trickWinner.playedCard);
                print("Perdedor: " + trickLoser + " con " + trickLoser.playedCard);
                for(int i = 0; i < players.Length; i++)
                {
                    if(players[i] == trickWinner)
                    {
                        currentPlayer = i;
                        break;
                    }
                }
            }
            else
            {
                print("Draw");
            }
            isNewTrickPlay = true;
            onTrickEnded.Raise();
            print("Trick ended");
        }
        if(!isGameEnded) CheckEnemyTurn();
    }
    
    private void DiscoverTriumphSuit()
    {
        print("Discovering triumph suit...");
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
        print("The triumph suit is: " + newTriumphSuit);
    }
}
