using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private Button resetButton;
    private Player player;
    private Player enemy;
    
    void Start()
    {
        endScreen.SetActive(false);
        player = GameManager.Instance.player;
        enemy = GameManager.Instance.enemy;
        player.OnPlayerDead += HandlePlayerDead;
        enemy.OnPlayerDead += HandleEnemyDead;
    }

    void OnDisable()
    {
        player.OnPlayerDead -= HandlePlayerDead;
        enemy.OnPlayerDead -= HandleEnemyDead;
    }

    public void OnResetButtonPressed()
    {
        endScreen.SetActive(false);
    }

    private void HandlePlayerDead()
    {
        endScreen.SetActive(true);
        endText.text = "YOU LOSE!";
    }

    private void HandleEnemyDead()
    {
        endScreen.SetActive(true);
        endText.text = "YOU WIN!";
    }

    
}
