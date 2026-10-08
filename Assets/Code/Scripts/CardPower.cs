using UnityEngine;

public class CardPower : MonoBehaviour
{
    private Card cardData;

    public void SetCardPowerData(Card newCardData)
    {
        cardData = newCardData;
    }

    public void InvokePower(Player trickWinner, Player trickLoser)
    {
        switch(cardData.GetCardSuit())
        {
            case Card.Suit.Clubs:
                ClubsPower(trickLoser);
                break;
            case Card.Suit.Cups:
                CupsPower(trickWinner);
                break;
            case Card.Suit.Golds:
                GoldsPower(trickWinner);
                break;
            case Card.Suit.Swords:
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
        
    }

    private void SwordsPower(Player trickLoser)
    {
        trickLoser.TakeDamage(1.0f);
    }

    private void GoldsPower(Player trickwinner)
    {
        
    }
}
