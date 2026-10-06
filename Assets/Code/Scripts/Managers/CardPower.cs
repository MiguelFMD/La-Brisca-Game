using UnityEngine;

[RequireComponent(typeof(CardDisplay))]
public class CardPower : MonoBehaviour
{
    private CardDisplay cardDisplay;
    private Card cardData;

    void Start()
    {
        cardDisplay = GetComponent<CardDisplay>();
        cardData = cardDisplay.GetCardData();
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
        
    }

    private void CupsPower(Player trickwinner)
    {
        
    }

    private void SwordsPower(Player trickLoser)
    {
        
    }

    private void GoldsPower(Player trickwinner)
    {
        
    }
}
