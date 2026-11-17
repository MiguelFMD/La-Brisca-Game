using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField]private Player player;
    [SerializeField]private TMP_Text maxHealth;
    [SerializeField]private TMP_Text currentHealth;
    [SerializeField]private TMP_Text shield;
    [SerializeField]private TMP_Text bleed;
    [SerializeField]private TMP_Text thorns;
    [SerializeField] private TMP_Text investment;
    [SerializeField]private TMP_Text cardsLeft;
    [SerializeField]private TMP_Text playedCard;
    

    // Update is called once per frame
    void Update()
    {
        maxHealth.text = "Max HP: " + player.state.MaxHealth;
        currentHealth.text = "Current HP: " + player.state.CurrentHealth;
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
        if(player.state.PlayedCard != null) playedCard.text = "Played card: " + player.state.PlayedCard.cardData;
        investment.text = "Investment: " + player.state.Investment;
    }
}
