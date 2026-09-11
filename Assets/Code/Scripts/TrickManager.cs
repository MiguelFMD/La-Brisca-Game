using System.Collections.Generic;
using UnityEngine;

public class TrickManager : MonoBehaviour
{
    private Card.Suit triumphSuit;
    private Card.Suit exitSuit;

    public void SetTriumphSuit(Card.Suit newSuit)
    {
        triumphSuit = newSuit;
    }

    public void SetExitSuit(Card.Suit newSuit)
    {
        exitSuit = newSuit;
    }

    public Player CalculateTrickWinner(List<Player> players, Card.Suit trickSuit)
    {
        List<Player> trickWinners = new List<Player>();
        //First look the cards that are triumph suit, those are possible winners
        foreach(Player player in players)
        {
            if(player.playedCard.cardSuit == triumphSuit)
            {
                trickWinners.Add(player);
            }
        }

        //If there is only 1, that's the winner
        if(trickWinners.Count == 1)
            return trickWinners[0];
        //if not just look for the biggest card
        else
        {
            Player winner = players[0];
            foreach(Player player in players)
            {
                //The card number is bigger
                if (winner.playedCard.number < player.playedCard.number)
                    winner = player;
                //The card numbers are equal
                else if(winner.playedCard.number == player.playedCard.number)
                {
                    //The winner is the card with the trickSuit
                    if(player.playedCard.cardSuit == trickSuit)
                        winner = player;
                }
            }
            return winner;
        }
    }

    
    private void RandomTriumphSuit()
    {
        SetTriumphSuit((Card.Suit)Random.Range(0, 3));
    }
    
}
