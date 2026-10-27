using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Listeners Events")]
    [SerializeField] private PlayerGameEvent onPlayerDeath;
    [SerializeField] private GameEvent onGameFinished;
    [SerializeField] private GameEvent onGameLoaded;
    [Header("Raise Events")]
    [SerializeField] private GameEvent onGameOver;
    [SerializeField] private GameEvent onGameRestarted;
    [SerializeField] private GameEvent onPlayerVictory;
    
    public Player player;
    public Player enemy;
    public Player[] players;
    public bool isGameEnded = false;

    void OnValidate()
    {
        if(onPlayerDeath == null) Debug.LogError($"[Critical] '{name}' needs 'onPlayerDeath' in order to work.", this);
        if(onGameFinished == null) Debug.LogError($"[Critical] '{name}' needs 'onGameFinished' in order to work.", this);
        if(onGameLoaded == null) Debug.LogError($"[Critical] '{name}' needs 'onGameLoaded' in order to work.", this);
    }
    void OnEnable()
    {
        if(onPlayerDeath != null) onPlayerDeath.OnRaised += HandlePlayerDeath;
        if(onGameFinished != null) onGameFinished.OnRaised += HandleOnGameFinished;
        if(onGameLoaded != null) onGameLoaded.OnRaised += HandleOnGameLoaded;
    }

    void OnDisable()
    {
        if(onPlayerDeath != null) onPlayerDeath.OnRaised -= HandlePlayerDeath;
        if(onGameFinished != null) onGameFinished.OnRaised -= HandleOnGameFinished;
        if(onGameLoaded != null) onGameLoaded.OnRaised -= HandleOnGameLoaded;
    }

    private void Awake()
    {
        players = new Player[2] {player, enemy};
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

    private void HandlePlayerDeath(Player deadPlayer)
    {
        isGameEnded = true;
        if(deadPlayer.isEnemy)
        {
            onPlayerVictory.Raise();
        }
        else
        {
            onGameOver.Raise();
        }
        
    }

    private void HandleOnGameFinished()
    {
        isGameEnded = true;
    }

    private void HandleOnGameLoaded()
    {
        isGameEnded = false;
    }
}
