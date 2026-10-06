using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrickManager : MonoBehaviour
{
    [SerializeField] private float cardAnimationDuration;
    [SerializeField] private AnimationCurve easingCurve;
    [SerializeField] private RectTransform centerTableTransform; //A basic rect transform in the main canvas indicating the center of the table
    private Player[] players;
    private Card.Suit triumphSuit;
    private Card.Suit trickSuit;
    private int currentPlayer;
    private bool isNewTrickPlay = true;
    private Player trickWinner;
    private Player trickLoser;
    private bool isAnimationPlaying = false;
    [SerializeField] private Image triumphCardDisplay;
    [SerializeField] private Card[] triumphCards;

    void OnEnable()
    {
        EventManager.OnAnimationEnded += HandleAnimationEnded;
    }

    void OnDisable()
    {
        EventManager.OnAnimationEnded -= HandleAnimationEnded;
    }

    //-----CORE FUNCTIONS------
    void Start()
    {
        players = GameManager.Instance.players;
        
    }

    //-----PUBLIC FUNCTIONS------
    public void CardPlayed(CardDisplay cardDisplay)
    {
        //If the card owner is the corresponding player and there is no card animation playing
        if(CheckPlayerTurn(cardDisplay.GetPlayerOwner()) && !isAnimationPlaying) 
        {
            CardToCenter(cardDisplay); //Put the card played on the center of the table
            cardDisplay.GetPlayerOwner().PlayCard(cardDisplay);
            if(isNewTrickPlay)
            {
                //In new tricks we set the new trick suit with the first played card
                if(players[currentPlayer].playedCard != null)
                {
                    SetTrickSuit(players[currentPlayer].playedCard.GetCardSuit());
                    isNewTrickPlay = false;
                    //print("Trick suit is: " + trickSuit);
                } 
                    
            }

        }
    }

    public void ResetTrick()
    {
        currentPlayer = Random.Range(0, players.Length);
        isNewTrickPlay = true;
        ClearTableVisuals();
        if(players[currentPlayer].isEnemy)
        {
            print("play card");
            players[currentPlayer].PlayRandomCard();
        }
        
    }
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
        List<Player> trickWinners = new List<Player>();
        //First look the cards that are triumph suit, those are possible winners
        for(int p = 0; p < players.Length; p++)
        {
            if(players[p].playedCard.GetCardSuit() == triumphSuit)
            {
                print("Jugador: " + players[p] + " tiene " + triumphSuit);
                trickWinners.Add(players[p]);
            }
        }

        //If there is only 1, that's the winner
        if(trickWinners.Count == 1)
        {
            print("Win by triumph suit");
            AddScoredCards(trickWinners[0], players);
            return trickWinners[0];
        }
        else
        {
            trickWinners.Clear();
            //if not lets check for the trickSuit
            for(int p = 0; p < players.Length; p++)
            {
                if(players[p].playedCard.GetCardSuit() == trickSuit)
                {
                    trickWinners.Add(players[p]);
                }
            }
            //If there is only 1, that's the winner
            if(trickWinners.Count == 1)
            {
                print("Win by trickSuit");
                AddScoredCards(trickWinners[0], players);
                return trickWinners[0];
            }
            
            else
            {
                Player winner = players[0];
                foreach(Player player in players)
                {
                    //The card rank is bigger
                    if (winner.playedCard.GetCardRank() < player.playedCard.GetCardRank())
                        winner = player;
                }
                print("Win by rank");
                AddScoredCards(winner, players);
                return winner;
            }
        }
    }
    /// <summary>
    /// Calculates the trick winner and loser (only 1vs1 version)
    /// </summary>
    /// <returns>True if someone wins, false if draw</returns>
    private bool CalculateTrickWinner1vs1()
    {
        //CARD SUIT
        if(players[0].playedCard.GetCardSuit() == triumphSuit)
        {
            if(players[1].playedCard.GetCardSuit() == triumphSuit)
            {
                if(players[0].playedCard.GetCardRank() > players[1].playedCard.GetCardRank()) //0 win
                {
                    trickWinner = players[0];
                    trickLoser = players[1];
                    return true;
                }
                else if(players[0].playedCard.GetCardRank() < players[1].playedCard.GetCardRank())//1 win
                {
                    trickWinner = players[1];
                    trickLoser = players[0];
                    return true;
                }
                else //Draw
                {
                    return false;
                }
            }
            else //0 win
            {
                trickWinner = players[0];
                trickLoser = players[1];
                return true;
            }
        }
        else
        {
            if(players[1].playedCard.GetCardSuit() == triumphSuit) //1 win
            {
                trickWinner = players[1];
                trickLoser = players[0];
                return true;
            }
            else //Miramos el trick suit
            {
                if(players[0].playedCard.GetCardSuit() == trickSuit)
                {
                    if(players[1].playedCard.GetCardSuit() == trickSuit)
                    {
                        if(players[0].playedCard.GetCardRank() > players[1].playedCard.GetCardRank()) //0 win
                        {
                            trickWinner = players[0];
                            trickLoser = players[1];
                            return true;
                        }
                        else if(players[0].playedCard.GetCardRank() < players[1].playedCard.GetCardRank())//1 win
                        {
                            trickWinner = players[1];
                            trickLoser = players[0];
                            return true;
                        }
                        else //Draw
                        {
                            return false;
                        }
                    }
                    else
                    {
                        trickWinner = players[0];
                        trickLoser = players[1];
                        return true;
                    }
                }
                else
                {
                    if(players[1].playedCard.GetCardSuit() == trickSuit)
                    {
                        trickWinner = players[1];
                        trickLoser = players[0];
                        return true;
                    }
                    else //Miramos cual es mas grande
                    {
                        if(players[0].playedCard.GetCardRank() > players[1].playedCard.GetCardRank()) //0 win
                        {
                            trickWinner = players[0];
                            trickLoser = players[1];
                            return true;
                        }
                        else if(players[0].playedCard.GetCardRank() < players[1].playedCard.GetCardRank())//1 win
                        {
                            trickWinner = players[1];
                            trickLoser = players[0];
                            return true;
                        }
                        else //Draw
                        {
                            return false;
                        }
                    }
                }
            }
        }
    }

    public void ClearTableVisuals()
    {
        CardDisplay[] cardsOnTable = centerTableTransform.GetComponentsInChildren<CardDisplay>();
        foreach(CardDisplay card in cardsOnTable)
        {
            GameManager.Instance.ReturnCardToPool(card);
        }
    }

    //-----PRIVATE FUNCTIONS------
    private void AddScoredCards(Player trickWinner, Player[] players)
    {
        foreach(Player player in players)
        {
            trickWinner.scoredCards.Add(player.playedCard);
        }
    }

    private bool CheckPlayerTurn(Player owner)
    {
        if(players[currentPlayer] != owner)
        {
            Debug.LogWarning("Wait for your turn!");
            return false;
        }
        else
            return true;
    }

    private void SelectNextPlayer()
    {
        if(currentPlayer >= players.Length - 1)
            currentPlayer = 0;
        else
            currentPlayer++;
    }

    private void CardToCenter(CardDisplay cardDisplay)
    {
        RectTransform cardRect = cardDisplay.GetComponent<RectTransform>();
        //cardRect.SetParent(centerTableTransform, true);
        cardRect.SetParent(cardRect.parent.parent);
        StartCoroutine(AnimateCard(cardRect));
        
    }

    private bool CheckAllPlayersHavePlayed()
    {
        foreach(Player player in players)
        {
            if(player.playedCard == null)
                return false;
        }
        //print("All players have played");
        return true;
    }

    private IEnumerator AnimateCard(RectTransform selectedCard)
    {
        //print("animo");
        isAnimationPlaying = true;
        Vector2 targetPosition = centerTableTransform.anchoredPosition;
        //print(targetPosition);
        Vector2 startPosition = selectedCard.anchoredPosition;
        float timeElapsed = 0f;

        while (timeElapsed < cardAnimationDuration)
        {
            timeElapsed += Time.deltaTime;
            float percentage = timeElapsed / cardAnimationDuration;
            
            // Evaluate the animation curve for smooth acceleration/deceleration
            float curveValue = easingCurve.Evaluate(percentage);
            
            selectedCard.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curveValue);
            yield return null; // Wait for the next frame
        }
        isAnimationPlaying = false;
        selectedCard.SetParent(centerTableTransform, true); // Snap to new parent
        EventManager.OnAnimationEnded?.Invoke();
    }

    private void HandleAnimationEnded()
    {
        SelectNextPlayer();   
        if(CheckAllPlayersHavePlayed())
        {
            //Player winner = CalculateTrickWinner();
            if(CalculateTrickWinner1vs1())
            {
                trickWinner.UseCardPower(trickLoser);
                trickLoser.TakeDamage(trickWinner.playedCard.CalculateCardValue());
                print("Ganador: " + trickWinner + " con " + trickWinner.playedCard);
                print("Perdedor: " + trickLoser + " con " + trickLoser.playedCard);
                //print("Triumph Suit era: " + triumphSuit);
                //print("Trick Suit era: " + trickSuit);
                //Assign the new currentPlayer (the winner)
                for(int i = 0; i < players.Length; i++)
                {
                    if(players[i] == trickWinner)
                    {
                        currentPlayer = i;
                        break;
                    }
                }
            }
            else
            {
                print("Draw");
            }
            ClearTableVisuals();
            isNewTrickPlay = true;
            
            //EventManager.OnTrickEnded?.Invoke();
            GameManager.Instance.HandleTrickEnded();
        }
        if(!GameManager.Instance.isGameEnded)
            if(players[currentPlayer].isEnemy)
            {
                players[currentPlayer].PlayRandomCard();
            }
    }
    
    public void DiscoverTriumphSuit()
    {
        //print("Discovering triumph suit...");
        Card.Suit newTriumphSuit = (Card.Suit)Random.Range(0, System.Enum.GetValues(typeof(Card.Suit)).Length);
        foreach(Card card in triumphCards)
        {
            if(card.GetCardSuit() == newTriumphSuit)
            {
                triumphCardDisplay.sprite = card.GetCardSprite();
                break; 
            }
        }
        triumphCardDisplay.gameObject.SetActive(true);
        SetTriumphSuit(newTriumphSuit);
        print("The triumph suit is: " + newTriumphSuit);
    }
}
