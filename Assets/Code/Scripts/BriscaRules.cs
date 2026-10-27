[System.Serializable]
public enum Suit
{
    Clubs,
    Cups,
    Golds,
    Swords
}
[System.Serializable]
public enum Rank
{
    Two,
    Four,
    Five,
    Six,
    Seven,
    Jack,
    Knight,
    King,
    Three,
    Ace
}

public class TrickResult
{
    public PlayerState winner;
    public PlayerState loser;
    public TrickResult(PlayerState trickWinner, PlayerState trickLoser)
    {
        winner = trickWinner;
        loser = trickLoser;
    }
}
public static class BriscaRules
{
    /// <summary>
    /// Calculates the trick winner and loser (only 1vs1 version)
    /// </summary>
    /// <returns>Returns the trick result. If there is draw returns null</returns>
    public static TrickResult CalculateTrickWinner(Suit triumphSuit,PlayerState firstPlayer, PlayerState secondPlayer)
    {
        CardData card0 = firstPlayer.PlayedCard;
        CardData card1 = secondPlayer.PlayedCard;
        Suit trickSuit = firstPlayer.PlayedCard.GetCardSuit();

        if (card0.GetCardSuit() == card1.GetCardSuit() && card0.GetCardRank() == card1.GetCardRank())
        {
            return null; // Draw
        }

        bool firstPlayerWins = false;

        if (card0.GetCardSuit() == triumphSuit && card1.GetCardSuit() == triumphSuit)
        {
            firstPlayerWins = card0.GetCardRank() > card1.GetCardRank();
        }
        else if (card0.GetCardSuit() == triumphSuit)
        {
            firstPlayerWins = true;
        }
        else if (card1.GetCardSuit() == triumphSuit)
        {
            firstPlayerWins = false;
        }
        else if (card0.GetCardSuit() == trickSuit && card1.GetCardSuit() == trickSuit)
        {
             firstPlayerWins = card0.GetCardRank() > card1.GetCardRank();
        }
        else if (card0.GetCardSuit() == trickSuit)
        {
            firstPlayerWins = true;
        }
        else if (card1.GetCardSuit() == trickSuit)
        {
            firstPlayerWins = false;
        }
        
        TrickResult trickResult;

        if (firstPlayerWins)
        {
            trickResult = new TrickResult(firstPlayer, secondPlayer);
        }
        else
        {
            trickResult = new TrickResult(secondPlayer, firstPlayer);    
        }

        return trickResult;
    }

    public static int CalculateCardValue(CardData cardData)
    {
        switch (cardData.GetCardRank())
        {
            case Rank.Jack:
                return 2;
            case Rank.Knight:
                return 3;
            case Rank.King:
                return 4;
            case Rank.Three:
                return 10;
            case Rank.Ace:
                return 11;
        }

        return 1;
    }

    public static bool CheckGameOverConditions(PlayerState firstPlayer, PlayerState secondPlayer)
    {
        if(firstPlayer.CurrentHealth <= 0 || secondPlayer.CurrentHealth <= 0)
            return true;
        if(CheckPlayersCardsLeft(firstPlayer, secondPlayer) != null)
            return true;
        return false;
    }

    private static PlayerState ComparePlayersHealth(PlayerState firstPlayer, PlayerState secondPlayer)
    {
        if(firstPlayer.CurrentHealth > secondPlayer.CurrentHealth)
            return firstPlayer;
        else
            return secondPlayer;
    }

    private static PlayerState CheckPlayersCardsLeft(PlayerState firstPlayer, PlayerState secondPlayer)
    {
        PlayerState winner = null;
        if(firstPlayer.HasNoCardsLeft())
        {
            if(secondPlayer.HasNoCardsLeft())
            {
                return ComparePlayersHealth(firstPlayer, secondPlayer); //DRAW
            }
            else //Solo el jugador se queda sin cartas
            {
                winner = secondPlayer;
            }
        }
        else if(secondPlayer.HasNoCardsLeft()) //Solo el enemigo se queda sin cartas
        {
            winner = firstPlayer;
        }
        return winner;
    }
}
