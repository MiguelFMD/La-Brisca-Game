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
    private Player trickWinner;
    private Player trickLoser;
    private CardData.Suit triumphSuit;
    private CardData.Suit trickSuit;
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
        print("Card selected");
        //If the card owner is the corresponding player and there is no card animation playing
        if(CheckPlayerTurn(cardData.owner) && !isAnimationPlaying) 
        {
            //CardToCenter(cardDisplay); //Put the card played on the center of the table
            cardData.owner.PlayCard(cardData);
            isAnimationPlaying = true;
            if(isNewTrickPlay)
            {
                //In new tricks we set the new trick suit with the first played card
                if(players[currentPlayer].playedCard != null)
                {
                    trickSuit = players[currentPlayer].playedCard.GetCardSuit();
                    isNewTrickPlay = false;
                    print("Trick suit is: " + trickSuit);
                } 
                    
            }
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
        print("Is enemy turn?");
        if(players[currentPlayer].isEnemy) //Is enemy turn?
        {
            print("Yes");
            players[currentPlayer].PlayRandomCard();
        }
        else print("No");
    }

    /// <summary>
    /// Calculates the trick winner and loser (only 1vs1 version)
    /// </summary>
    /// <returns>True if someone wins, false if draw</returns>
    private bool CalculateTrickWinner1vs1()
    {
        CardData card0 = players[0].playedCard;
        CardData card1 = players[1].playedCard;

        // Comprobamos el empate exacto primero. Si son idénticas en rango y palo, es empate.
        if (card0.GetCardSuit() == card1.GetCardSuit() && card0.GetCardRank() == card1.GetCardRank())
        {
            return false; // Empate
        }

        bool player0Wins = false;

        // CASO 1: Ambos juegan el palo de triunfo
        if (card0.GetCardSuit() == triumphSuit && card1.GetCardSuit() == triumphSuit)
        {
            player0Wins = card0.GetCardRank() > card1.GetCardRank();
        }
        // CASO 2: Solo el jugador 0 juega triunfo
        else if (card0.GetCardSuit() == triumphSuit)
        {
            player0Wins = true;
        }
        // CASO 3: Solo el jugador 1 juega triunfo
        else if (card1.GetCardSuit() == triumphSuit)
        {
            player0Wins = false;
        }
        // CASO 4: Ninguno juega triunfo. Ambos juegan el trickSuit (el palo que inició la baza)
        else if (card0.GetCardSuit() == trickSuit && card1.GetCardSuit() == trickSuit)
        {
             player0Wins = card0.GetCardRank() > card1.GetCardRank();
        }
        // CASO 5: Solo el jugador 0 juega el trickSuit (el jugador 1 tiró otro palo que no es triunfo)
        else if (card0.GetCardSuit() == trickSuit)
        {
            player0Wins = true;
        }
        // CASO 6: Solo el jugador 1 juega el trickSuit (el jugador 0 tiró otro palo que no es triunfo)
        else if (card1.GetCardSuit() == trickSuit)
        {
            player0Wins = false;
        }

        // Asignamos ganadores y perdedores basados en el resultado
        if (player0Wins)
        {
            trickWinner = players[0];
            trickLoser = players[1];
        }
        else
        {
            trickWinner = players[1];
            trickLoser = players[0];
        }

        return true;
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
            //Player winner = CalculateTrickWinner();
            if(CalculateTrickWinner1vs1())
            {
                trickWinner.UseCardPower(trickLoser);
                trickLoser.TakeDamage(trickWinner.playedCard.CalculateCardValue());
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
        CardData.Suit newTriumphSuit = (CardData.Suit)Random.Range(0, System.Enum.GetValues(typeof(CardData.Suit)).Length);
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
