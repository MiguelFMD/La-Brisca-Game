using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Listeners Events")]
    [SerializeField] private PlayerGameEvent onPlayerDeath;
    [SerializeField] private GameEvent onTrickEnded;
    [SerializeField] private CardGameEvent onReturnCard;
    [SerializeField] private GameEvent onGameFinished;
    [SerializeField] private GameEvent onGameLoaded;
    [Header("Raise Events")]
    [SerializeField] private GameEvent onGameOver;
    [SerializeField] private GameEvent onGameRestarted;
    [SerializeField] private GameEvent onPlayerVictory;
    
    public Player player;
    public Player enemy;
    public Player[] players;

    //Card prefab and pool
    [SerializeField] private GameObject cardDisplayPrefab;
    private Queue<CardDisplay> cardPool = new Queue<CardDisplay>();
    [SerializeField] private RectTransform cardPoolTransform;

    public bool isGameEnded = false;

    void OnValidate()
    {
        //if(onGameRestarted == null) Debug.LogError($"[Critical] '{name}' needs 'onGameRestarted' in order to work.", this);
        if(onReturnCard == null) Debug.LogError($"[Critical] '{name}' needs 'onReturnCard' in order to work.", this);
        if(onPlayerDeath == null) Debug.LogError($"[Critical] '{name}' needs 'onPlayerDeath' in order to work.", this);
        if(onTrickEnded == null) Debug.LogError($"[Critical] '{name}' needs 'onTrickEnded' in order to work.", this);
        if(onGameOver == null) Debug.LogError($"[Critical] '{name}' needs 'onGameOver' in order to work.", this);
        if(onPlayerVictory == null) Debug.LogError($"[Critical] '{name}' needs 'onPlayerVictory' in order to work.", this);
        if(onGameFinished == null) Debug.LogError($"[Critical] '{name}' needs 'onGameFinished' in order to work.", this);
        if(onGameLoaded == null) Debug.LogError($"[Critical] '{name}' needs 'onGameLoaded' in order to work.", this);
    }
    void OnEnable()
    {
        players = new Player[2] {player, enemy};
        if(onReturnCard != null) onReturnCard.OnRaised += ReturnCardToPool;
        if(onPlayerDeath != null) onPlayerDeath.OnRaised += HandlePlayerDeath;
        if(onTrickEnded != null) onTrickEnded.OnRaised += HandleTrickEnded;
        if(onGameFinished != null) onGameFinished.OnRaised += HandleOnGameFinished;
        if(onGameLoaded != null) onGameLoaded.OnRaised += HandleOnGameLoaded;
    }

    void OnDisable()
    {
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

    private void ResetGame()
    {
        isGameEnded = false;
        onGameRestarted.Raise();
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
        print("Comprobar final de partida");
        if(player.IsHandEmpty() && player.IsDeckEmpty())
        {
            if(enemy.IsHandEmpty() && enemy.IsDeckEmpty())
            {
                //GetWinnerByHealth().TriggerPlayerDeath();
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
            onPlayerVictory.Raise();
            print("YOU WIN");
        }
        else
        {
            onGameOver.Raise();
            print("YOU LOSE");
        }
        
    }

    private void HandleOnGameFinished()
    {
        isGameEnded = true;
        print("GAME FINISHED");
    }

    private void HandleOnGameLoaded()
    {
        isGameEnded = false;
    }
}
