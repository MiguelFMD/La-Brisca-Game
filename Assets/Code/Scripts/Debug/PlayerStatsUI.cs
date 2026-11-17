using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField]private TMP_Text maxHealth;
    [SerializeField]private TMP_Text currentHealth;
    [SerializeField]private TMP_Text shield;
    [SerializeField]private TMP_Text effects;
    [SerializeField]private Player player;

    // Update is called once per frame
    void Update()
    {
        maxHealth.text = "Max HP: " + player.maxHealth;
        currentHealth.text = "Current HP: " + player.currentHealth;
        shield.text = "Shield: " + player.state.CurrentShield;
    }
}
