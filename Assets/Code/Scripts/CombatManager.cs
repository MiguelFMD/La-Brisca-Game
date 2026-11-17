
public static class CombatManager
{
    public static void ApplyWinnerPower(PlayerState winner, PlayerState loser)
    {
        CardData winnerCard = winner.PlayedCard.cardData;
        Power winnerPower = null;
        switch(winnerCard.GetCardSuit())
        {
            case Suit.Swords:
                winnerPower = new PowerDamage();
                break;
            case Suit.Clubs:
                winnerPower = new PowerShield();
                break;
            case Suit.Cups:
                winnerPower = new PowerHeal();
                break;
            case Suit.Golds:
                winnerPower = new PowerBurnCards();
                break;
        }
        winnerPower.Activate(winnerCard, winner, loser);
    }

    public static void ApplyLoserPower(PlayerState loser, PlayerState winner)
    {
        CardData loserCard = loser.PlayedCard.cardData;
        Power loserPower = null;
        switch(loserCard.GetCardSuit())
        {
            case Suit.Swords:
                loserPower = new PowerBleed();
                break;
            case Suit.Clubs:
                loserPower = new PowerThorns();
                break;
            case Suit.Cups:
                loser.ClearStatus();
                break;
            case Suit.Golds:
                loserPower = new PowerBurnCards();
                break;
        }
        loserPower.Activate(loserCard, loser, winner);
    }
}
