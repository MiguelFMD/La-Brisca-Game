using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int currentLevel = 0;
    [Tooltip("Each enemy is a level. Put as much enemies as levels wanted")]
    [SerializeField] private EnemyType [] enemies;
    [SerializeField] private Player enemy; 
    [SerializeField] private GameEvent onGameLoaded;
    [SerializeField] private GameEvent onNextLevelStarted;
    [SerializeField] private GameEvent onGameRestarted;

    void OnValidate()
    {
        if(onGameLoaded == null) Debug.LogError($"[Critical] '{name}' needs 'onGameLoaded' in order to work.", this);
    }

    void OnEnable()
    {
        if(onGameRestarted != null) onGameRestarted.OnRaised += HandleOnRestarted;
        if(onNextLevelStarted != null) onNextLevelStarted.OnRaised += HandleOnNextLevel;
    }

    private void HandleOnNextLevel()
    {
        if(currentLevel < enemies.Length)
        {
            currentLevel++;
            LoadNewEnemy();  
        }
    }

    private void HandleOnRestarted()
    {
        currentLevel = 0;
        LoadNewEnemy();
    }

    private void LoadNewEnemy()
    {
        enemy.SetPlayerData(enemies[currentLevel].deck, enemies[currentLevel].health);
        onGameLoaded.Raise();
    }
}
