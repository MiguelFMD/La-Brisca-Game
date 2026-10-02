using UnityEngine;

[RequireComponent(typeof(Player))]
public class EnemyManager : MonoBehaviour
{
    Player enemyPlayer;
    void Start()
    {
        enemyPlayer = GetComponent<Player>();
        enemyPlayer.isEnemy = true;
    }

    public void PlayRandomCard()
    {
        enemyPlayer.PlayRandomCard();
    }
}
