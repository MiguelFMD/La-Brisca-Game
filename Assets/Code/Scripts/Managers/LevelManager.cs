using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int currentLevel = 0;
    [Tooltip("Each enemy is a level. Put as much enemies as levels wanted")] 
    [SerializeField] private EnemyType [] enemies;
    void OnEnable()
    {
        EventManager.OnNextRoundButtonClicked += HandleOnGameEnded;
    }
    void OnDisable()
    {
        EventManager.OnNextRoundButtonClicked -= HandleOnGameEnded;
    }
    private void HandleOnGameEnded()
    {
        if(currentLevel < enemies.Length)
        {
            currentLevel++;
            LoadNewEnemy();  
        }
           
        //FALTA  --> Añadir al deck del jugador las nuevas cartas
       
    }

    private void LoadNewEnemy()
    {
        GameManager.Instance.PrepareNextRound(enemies[currentLevel].deck, enemies[currentLevel].health);
    }
}
