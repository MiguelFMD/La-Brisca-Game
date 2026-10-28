using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Tooltip("Each level is an enemy. Put as much levels as levels wanted")]
    [SerializeField] private EnemyType [] levels;
    [Header("Raise Events")]
    [SerializeField] private GameEvent onGameLoaded;
    [SerializeField] private GameEvent onGameFinished;
    
    [Header("Listener Events")]
    [SerializeField] private GameEvent onGameRestarted;
    [SerializeField] private GameEvent onNextLevelStarted;
    [SerializeField] private GameEvent onPlayerVictory;
    private Player enemy;
    private int currentLevel = 0;

    void OnValidate()
    {
        if(onGameLoaded == null) Debug.LogError($"[Critical] '{name}' needs 'onGameLoaded' in order to work.", this);
        if(onNextLevelStarted == null) Debug.LogError($"[Critical] '{name}' needs 'onNextLevelStarted' in order to work.", this);
        if(onGameRestarted == null) Debug.LogError($"[Critical] '{name}' needs 'onGameRestarted' in order to work.", this);
        if(onPlayerVictory == null) Debug.LogError($"[Critical] '{name}' needs 'onPlayerVictory' in order to work.", this);
        if(onGameFinished == null) Debug.LogError($"[Critical] '{name}' needs 'onGameFinished' in order to work.", this);
    }

    void OnEnable()
    {
        if(onGameRestarted != null) onGameRestarted.OnRaised += HandleOnRestarted;
        if(onNextLevelStarted != null) onNextLevelStarted.OnRaised += HandleOnNextLevel;
        if(onPlayerVictory != null) onPlayerVictory.OnRaised += HandleOnPlayerVictory;
    }

    void OnDisable()
    {
        if(onGameRestarted != null) onGameRestarted.OnRaised -= HandleOnRestarted;
        if(onNextLevelStarted != null) onNextLevelStarted.OnRaised -= HandleOnNextLevel;
        if(onPlayerVictory != null) onPlayerVictory.OnRaised -= HandleOnPlayerVictory;
    }

    void Awake()
    {
        if(levels.Length < 0) Debug.LogError($"[Critical] '{name}' needs 'levels/enemies' in order to work.", this);
        
        Player playerEnemy = GameObject.FindWithTag("Enemy").GetComponent<Player>();
        if(playerEnemy != null)
            enemy = playerEnemy;
        else
            Debug.LogError("Enemy not found in scene");
    }

    private void HandleOnNextLevel()
    {
        print("---------------NEXT LEVEL----------------");
        LoadNewEnemy();
    }

    private void HandleOnPlayerVictory()
    {
        if(currentLevel < levels.Length - 1)
        {
            currentLevel++;
        }
        else
        {
            onGameFinished.Raise();
        }
    }

    private void HandleOnRestarted()
    {
        currentLevel = 0;
        LoadNewEnemy();
    }

    private void LoadNewEnemy()
    {
        if(levels.Length == 0) return;
        enemy.SetPlayerData(levels[currentLevel].deck, levels[currentLevel].health);
        onGameLoaded.Raise();
    }
}
