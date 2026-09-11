using System.Collections.Generic;
using UnityEngine;

public class TrickManager : MonoBehaviour
{
    private Card.Suit triumphSuit;
    private Card.Suit trickSuit;

    public void SetTriumphSuit(Card.Suit newSuit)
    {
        triumphSuit = newSuit;
    }

    public void SetTrickSuit(Card.Suit newSuit)
    {
        trickSuit = newSuit;
    }

    public Player CalculateTrickWinner()
    {
        Player[] players = GameManager.Instance.players;
        List<Player> trickWinners = new List<Player>();
        //First look the cards that are triumph suit, those are possible winners
        for(int p = 0; p < players.Length; p++)
        {
            if(players[p].playedCard.cardSuit == triumphSuit)
            {
                trickWinners.Add(players[p]);
            }
        }

        //If there is only 1, that's the winner
        if(trickWinners.Count == 1)
            return trickWinners[0];
        else
        {
            trickWinners.Clear();
            //if not lets check for the trickSuit
            for(int p = 0; p < players.Length; p++)
            {
                if(players[p].playedCard.cardSuit == trickSuit)
                {
                    trickWinners.Add(players[p]);
                }
            }
            //If there is only 1, that's the winner
            if(trickWinners.Count == 1)
                return trickWinners[0];
            else
            {
                Player winner = players[0];
                foreach(Player player in players)
                {
                    //The card number is bigger
                    if (winner.playedCard.number < player.playedCard.number)
                        winner = player;
                }
                return winner;
            }
        }
    }

    
    private void RandomTriumphSuit()
    {
        SetTriumphSuit((Card.Suit)Random.Range(0, 3));
    }
    
}
