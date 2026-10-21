using System;
using Unity.VisualScripting;
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
    [SerializeField] private GameEvent onPlayerVictory;
    [SerializeField] private GameEvent onGameFinished;

    void OnValidate()
    {
        if(onGameLoaded == null) Debug.LogError($"[Critical] '{name}' needs 'onGameLoaded' in order to work.", this);
        if(onPlayerVictory == null) Debug.LogError($"[Critical] '{name}' needs 'onPlayerVictory' in order to work.", this);
    }

    void OnEnable()
    {
        if(onGameRestarted != null) onGameRestarted.OnRaised += HandleOnRestarted;
        if(onNextLevelStarted != null) onNextLevelStarted.OnRaised += HandleOnNextLevel;
        if(onPlayerVictory != null) onPlayerVictory.OnRaised += HandleOnPlayerVictory;
    }

    private void HandleOnNextLevel()
    {
        print("---------------NEXT LEVEL----------------");
        LoadNewEnemy();
    }

    private void HandleOnPlayerVictory()
    {
        print("current level is: " + currentLevel);
        if(currentLevel < enemies.Length)
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
        enemy.SetPlayerData(enemies[currentLevel].deck, enemies[currentLevel].health);
        onGameLoaded.Raise();
    }
}
