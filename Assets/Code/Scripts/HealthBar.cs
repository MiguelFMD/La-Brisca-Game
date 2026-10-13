using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Image))]
public class HealthBar : MonoBehaviour
{
    private Image healthBar;
    void OnEnable()
    {
        healthBar = GetComponent<Image>();
    }

    public void HandlePlayerHealthChanged(Player player)
    {
        ChangeHealthBar(player.currentHealth, player.maxHealth);
    }

    private void ChangeHealthBar(float current, float max)
    {
        float normalizedHealth = current / max;
        healthBar.material.SetFloat("_Health", normalizedHealth);
    }
}
