using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField]private Player player;
    [SerializeField]private TMP_Text health;
    [SerializeField]private TMP_Text cardsLeft;
    [SerializeField]private TMP_Text shield;
    [SerializeField]private TMP_Text bleed;
    [SerializeField]private TMP_Text thorns;
    [SerializeField] private TMP_Text investment;


    [Header("Listener events")]
    [SerializeField] private PlayerGameEvent onPlayerHealthChanged;
    [SerializeField] private PlayerGameEvent onDeckCardRemoved;
    [SerializeField] private PlayerGameEvent onStatusEffectChanged;
    [SerializeField] private PlayerGameEvent onInvestmentChanged;
    [SerializeField] private PlayerGameEvent onShieldChanged;

    private void OnEnable()
    {
        onPlayerHealthChanged.OnRaised += UpdatePlayerHealth;
        onDeckCardRemoved.OnRaised += UpdateCardsLeft;
        onStatusEffectChanged.OnRaised += UpdateStatusEffects;
        onInvestmentChanged.OnRaised += UpdateInvestment;
        onShieldChanged.OnRaised += UpdateShield;
    }

    private void OnDisable()
    {
        onPlayerHealthChanged.OnRaised -= UpdatePlayerHealth;
        onDeckCardRemoved.OnRaised -= UpdateCardsLeft;
        onStatusEffectChanged.OnRaised -= UpdateStatusEffects;
        onInvestmentChanged.OnRaised -= UpdateInvestment;
        onShieldChanged.OnRaised -= UpdateShield;
    }

    void Start()
    {
        UpdateCardsLeft(player);
        UpdatePlayerHealth(player);
        UpdateInvestment(player);
        UpdateStatusEffects(player);
        UpdateShield(player);
    }

    public void UpdatePlayerHealth(Player other)
    {
        if(other != player) return;
        health.text = player.state.CurrentHealth + " / " + player.state.MaxHealth;
    }

    public void UpdateCardsLeft(Player other)
    {
        cardsLeft.text = player.state.Deck.CountCards().ToString();
    }

    public void UpdateStatusEffects(Player other)
    {
        if(player.state.activeStatusEffects.Count == 0)
        {
            thorns.text = ":0";
            bleed.text = ":0";
        }
        foreach(StatusEffect statusEffect in player.state.activeStatusEffects)
        {
            if(statusEffect is ThornsEffect) thorns.text = ":" + statusEffect.Stacks;
            else if(statusEffect is BleedingEffect) bleed.text = ":" + statusEffect.Stacks;
        }
    }
    public void UpdateInvestment(Player other)
    {
        investment.text = ":" + player.state.Investment;
    }
    public void UpdateShield(Player other)
    {
        shield.text = ":" + player.state.CurrentShield;
    }
}
