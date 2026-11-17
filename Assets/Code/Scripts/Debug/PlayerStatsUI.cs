using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Scene references")]
    [SerializeField]private Player player;
    [SerializeField]private TMP_Text health;
    [SerializeField]private TMP_Text cardsLeft;


    [Header("Listener events")]
    [SerializeField] private PlayerGameEvent onPlayerHealthChanged;
    [SerializeField] private PlayerGameEvent onDeckCardRemoved;
    /*[SerializeField]private TMP_Text shield;
    [SerializeField]private TMP_Text bleed;
    [SerializeField]private TMP_Text thorns;
    [SerializeField] private TMP_Text investment;*/

    private void OnEnable()
    {
        onPlayerHealthChanged.OnRaised += UpdatePlayerHealth;
        onDeckCardRemoved.OnRaised += UpdateCardsLeft;
    }

    private void OnDisable()
    {
        onPlayerHealthChanged.OnRaised -= UpdatePlayerHealth;
        onDeckCardRemoved.OnRaised -= UpdateCardsLeft;
    }

    void Start()
    {
        UpdateCardsLeft(player);
    }

    // Update is called once per frame
    /*void Update()
    {
        shield.text = "Shield: " + player.state.CurrentShield;
        if(player.state.activeStatusEffects.Count == 0)
        {
            thorns.text = "Thorns: 0";
            bleed.text = "Bleed: 0";
        }
        foreach(StatusEffect statusEffect in player.state.activeStatusEffects)
        {
            if(statusEffect is ThornsEffect) thorns.text = "Thorns: " + statusEffect.Stacks;
            else if(statusEffect is BleedingEffect) bleed.text = "Bleed: " + statusEffect.Stacks;
        }
        cardsLeft.text = "Cards: " + player.state.Deck.CountCards();
        //if(player.state.PlayedCard != null) playedCard.text = "Played card: " + player.state.PlayedCard.cardData;
        investment.text = "Investment: " + player.state.Investment;
    }*/

    public void UpdatePlayerHealth(Player other)
    {
        if(other != player) return;
        health.text = player.state.CurrentHealth + " / " + player.state.MaxHealth;
    }

    public void UpdateCardsLeft(Player other)
    {
        cardsLeft.text = player.state.Deck.CountCards().ToString();
    }
}
