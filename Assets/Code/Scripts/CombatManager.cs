
public static class CombatManager
{
    public static void ApplyWinnerPower(Player winner, Player loser)
    {
        CardData winnerCard = winner.playedCard.cardData;
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

    public static void ApplyLoserPower(Player loser, Player winner)
    {
        CardData loserCard = loser.playedCard.cardData;
        Power loserPower = null;
        switch(loserCard.GetCardSuit())
        {
            case Suit.Swords:
                loserPower = new PowerDamage();
                break;
            case Suit.Clubs:
                loserPower = new PowerShield();
                break;
            case Suit.Cups:
                loserPower = new PowerHeal();
                break;
            case Suit.Golds:
                loserPower = new PowerBurnCards();
                break;
        }
        loserPower.Activate(loserCard, loser, winner);
    }
}
