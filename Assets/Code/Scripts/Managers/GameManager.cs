using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Listeners Events")]
    [SerializeField] private GameEvent onGameRestarted;
    [SerializeField] private PlayerGameEvent onPlayerDeath;
    [SerializeField] private GameEvent onTrickEnded;
    [SerializeField] private CardGameEvent onReturnCard;
    [Header("Raise Events")]
    [SerializeField] private GameEvent onGameOver;
    
    public Player player;
    public Player enemy;
    public Player[] players;

    //Card prefab and pool
    [SerializeField] private GameObject cardDisplayPrefab;
    private Queue<CardDisplay> cardPool = new Queue<CardDisplay>();
    [SerializeField] private RectTransform cardPoolTransform;

    public bool isGameEnded = false;
    
    
    void OnEnable()
    {
        players = new Player[2] {player, enemy};
        if(onGameRestarted != null) onGameRestarted.OnRaised += ResetGame;
        if(onReturnCard != null) onReturnCard.OnRaised += ReturnCardToPool;
        if(onPlayerDeath != null) onPlayerDeath.OnRaised += HandlePlayerDeath;
        if(onTrickEnded != null) onTrickEnded.OnRaised += HandleTrickEnded;
    }

    void OnDisable()
    {
        if(onGameRestarted != null) onGameRestarted.OnRaised -= ResetGame;
        if(onReturnCard != null) onReturnCard.OnRaised -= ReturnCardToPool;
        if(onPlayerDeath != null) onPlayerDeath.OnRaised -= HandlePlayerDeath;
        if(onTrickEnded != null) onTrickEnded.OnRaised -= HandleTrickEnded;
    }

    private void Awake()
    {
        // 1. Verificar si ya existe una instancia
        if (Instance != null && Instance != this)
        {
            // Si ya existe otra, destruir este duplicado
            Destroy(gameObject);
            return;
        }

        // 2. Asignar la instancia actual
        Instance = this;

        // 3. Hacer que persista entre cambios de escena
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
       ResetGame();
    }

    public void ResetGame()
    {
        isGameEnded = false;
    }

    public void PrepareNextRound(DeckType enemyDeck, float enemyMaxHealth)
    {
        enemy.SetPlayerData(enemyDeck, enemyMaxHealth);
        ResetGame();
    }

    public CardDisplay GetCardVisual()
    {
        if (cardPool.Count > 0)
        {
            CardDisplay pooledCard = cardPool.Dequeue();
            pooledCard.gameObject.SetActive(true);
            return pooledCard;
        }
        else
        {
            //print("no habian cartas");
            // Si no hay cartas reciclables, creamos una nueva
            GameObject newCard = Instantiate(cardDisplayPrefab);
            return newCard.GetComponent<CardDisplay>();
        }
    }

    private void ReturnCardToPool(CardDisplay cardToReturn)
    {
        cardToReturn.gameObject.SetActive(false); // Hide the card
        cardPool.Enqueue(cardToReturn); // Save it for future
        cardToReturn.transform.SetParent(cardPoolTransform);
    }

    public void HandleTrickEnded()
    {
        RemovePlayersPlayedCard(); //Remove the played cards from players
        print("Comprobar final de partida");
        if(player.IsHandEmpty() && player.IsDeckEmpty())
        {
            if(enemy.IsHandEmpty() && enemy.IsDeckEmpty())
            {
                enemy.TriggerPlayerDeath();
                Debug.LogWarning("DRAW");
            }
            else //Solo el jugador se queda sin cartas
            {
                player.TriggerPlayerDeath();
            }
            isGameEnded = true;
        }
        else if(enemy.IsHandEmpty()) //Solo el enemigo se queda sin cartas
        {
            enemy.TriggerPlayerDeath();
            isGameEnded = true;
        }
        DrawOneCard();
    }

    

    private void DrawOneCard()
    {
        foreach(Player player in players)
        {
            player.DrawOneCard();
        }
    }

    private void RemovePlayersPlayedCard()
    {
        print("Removing played cards from players...");
        foreach(Player player in players)
        {
            player.ClearPlayedCard();
        }
    }


    private Player GetWinnerByScore() //By Score (old)
    {
        Player winner = players[0];
        int winnerScore = players[0].CalculateScore();
        //print("Player " + winner.playerNumber + " scored " + winnerScore);
        for(int p = 1; p < players.Length; p++)
        {
            int otherScore = players[p].CalculateScore();
            if(winnerScore < otherScore)
            {
                winnerScore = otherScore;
                winner = players[p];
            }
            //print("Player " + players[p].playerNumber + " scored " + otherScore);
        }
        print("The winner is: " + winner.name);
        return winner;
    }

    private Player GetWinnerByHealth()
    {
        if(player.currentHealth > enemy.currentHealth)
            return player;
        else if(player.currentHealth < enemy.currentHealth)
            return enemy;
        else
            return GetWinnerByScore();
    }

    private void HandlePlayerDeath(Player deadPlayer)
    {
        isGameEnded = true;
        if(deadPlayer.isEnemy)
        {
            print("YOU WIN");
        }
        else
        {
            print("YOU LOSE");
        }
    }
}
