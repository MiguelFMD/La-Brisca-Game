using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int currentLevel = 0;
    [Tooltip("Each enemy is a level. Put as much enemies as levels wanted")] 
    [SerializeField] private EnemyType [] enemies;
    private void HandleOnGameEnded()
    {
        if(currentLevel < enemies.Length)
        {
            currentLevel++;
            LoadNewEnemy();  
        }
    }

    private void LoadNewEnemy()
    {
        GameManager.Instance.PrepareNextRound(enemies[currentLevel].deck, enemies[currentLevel].health);
    }
}
