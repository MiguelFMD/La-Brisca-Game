using UnityEngine;

public class EndScreenUI : MonoBehaviour
{
    [Header("Open Events")]
    [SerializeField] private GameEvent onGameOver;

    [Header("Close Events")]
    [SerializeField] private GameEvent onGameRestarted;
    [SerializeField] private GameEvent onReturnToMainMenu;
    [SerializeField] private GameEvent onNextLevelStarted;

    void OnValidate()
    {
        if (onGameOver == null) Debug.LogError($"[Critical] '{name}' needs 'onGameOver' in order to work.", this);
    }

    private void OnEnable()
    {
        // Escuchar apertura
        if (onGameOver != null) onGameOver.OnRaised += Show;

        // Escuchar todos los eventos que cierran la pantalla
        if (onGameRestarted != null) onGameRestarted.OnRaised += Hide;
        if (onReturnToMainMenu != null) onReturnToMainMenu.OnRaised += Hide;
        if (onNextLevelStarted != null) onNextLevelStarted.OnRaised += Hide;
    }

    private void OnDisable()
    {
        if (onGameOver != null) onGameOver.OnRaised -= Show;
        if (onGameRestarted != null) onGameRestarted.OnRaised -= Hide;
        if (onReturnToMainMenu != null) onReturnToMainMenu.OnRaised -= Hide;
        if (onNextLevelStarted != null) onNextLevelStarted.OnRaised -= Hide;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}