using UnityEngine;

public class CardPower : MonoBehaviour
{
    private CardData cardData;

    public void SetCardPowerData(CardData newCardData)
    {
        cardData = newCardData;
    }

    public void InvokePower(Player trickWinner, Player trickLoser)
    {
        switch(cardData.GetCardSuit())
        {
            case CardData.Suit.Clubs:
                ClubsPower(trickLoser);
                break;
            case CardData.Suit.Cups:
                CupsPower(trickWinner);
                break;
            case CardData.Suit.Golds:
                GoldsPower(trickWinner);
                break;
            case CardData.Suit.Swords:
                SwordsPower(trickLoser);
                break;
                
        }
    }

    private void ClubsPower(Player trickLoser)
    {
        trickLoser.RemoveRandomCard();
    }

    private void CupsPower(Player trickwinner)
    {
        trickwinner.Heal(2.0f);
    }

    private void SwordsPower(Player trickLoser)
    {
        trickLoser.TakeDamage(1.0f);
    }

    private void GoldsPower(Player trickwinner)
    {
        trickwinner.DrawOneCard();
    }
}
