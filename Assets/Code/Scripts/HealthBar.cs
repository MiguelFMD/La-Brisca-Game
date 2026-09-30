using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (Image))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Player targetPlayer;
    private Image healthBar;
    void OnEnable()
    {
        if(targetPlayer != null)
            targetPlayer.OnPlayerHealthChanged += HandlePlayerHealthChanged;
        healthBar = GetComponent<Image>();
    }

    void OnDisable()
    {
        if(targetPlayer != null)
            targetPlayer.OnPlayerHealthChanged -= HandlePlayerHealthChanged;
    }

    private void HandlePlayerHealthChanged(float current, float max)
    {
        float normalizedHealth = current / max;
        healthBar.material.SetFloat("_Health", normalizedHealth);
    }
}
