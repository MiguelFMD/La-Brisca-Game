using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrickManager : MonoBehaviour
{
    [Header("Card animation")]
    [SerializeField] private float cardAnimationDuration;
    [SerializeField] private AnimationCurve easingCurve;
    [Header("Table references")]
    [SerializeField] private Image triumphCardDisplay;
    [SerializeField] private Card[] triumphCards;
    [SerializeField] private RectTransform centerTableTransform; //A basic rect transform in the main canvas indicating the center of the table
    [Header("Listeners Events")]
    [SerializeField] private GameEvent onGameRestarted;
    [SerializeField] private CardGameEvent onCardSelected;
    [SerializeField] private GameEvent onCardsDealt;
    [Header("Raise Events")]
    [SerializeField] private GameEvent onTrickEnded;
    [SerializeField] private GameEvent onCardAnimationEnded;
    [SerializeField] private Player[] players;
    private Player trickWinner;
    private Player trickLoser;
    private Card.Suit triumphSuit;
    private Card.Suit trickSuit;
    private int currentPlayer;
    private bool isNewTrickPlay = true;
    private bool isAnimationPlaying = false;

    //-----CORE FUNCTIONS------
    void OnValidate()
    {
        if(onGameRestarted == null) Debug.LogError($"[Critical] '{name}' needs 'onGameRestarted' in order to work.", this);
        if(onCardSelected == null) Debug.LogError($"[Critical] '{name}' needs 'onCardSelected' in order to work.", this);
        if(onCardsDealt == null) Debug.LogError($"[Critical] '{name}' needs 'onCardsDealt' in order to work.", this);
    }
    void OnEnable()
    {
        if(onGameRestarted != null) onGameRestarted.OnRaised += ResetTrick;
        if(onCardSelected != null) onCardSelected.OnRaised += CardSelected;
        if(onCardsDealt != null) onCardsDealt.OnRaised += HandleCardsDealt;
    }
    void OnDisable()
    {
        if(onGameRestarted != null) onGameRestarted.OnRaised -= ResetTrick;
        if(onCardSelected != null) onCardSelected.OnRaised -= CardSelected;
        if(onCardsDealt != null) onCardsDealt.OnRaised -= HandleCardsDealt;
    }

    private void ResetTrick()
    {
        currentPlayer = Random.Range(0, players.Length);
        isNewTrickPlay = true;
        ClearTableVisuals();
        DiscoverTriumphSuit();
        //CheckEnemyTurn();
    }

    private void CardSelected(CardDisplay cardDisplay)
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
                    trickSuit = players[currentPlayer].playedCard.GetCardSuit();
                    isNewTrickPlay = false;
                    print("Trick suit is: " + trickSuit);
                } 
                    
            }
        }
    }

    private int canStartTrick = 0;
    private void HandleCardsDealt()
    {
        canStartTrick++;
        if(canStartTrick == 2)
            CheckEnemyTurn();
    }

    private void CheckEnemyTurn()
    {
        print("Is enemy turn?");
        if(players[currentPlayer].isEnemy) //Is enemy turn?
        {
            print("Yes");
            CardToCenter(players[currentPlayer].PlayRandomCard());
        }
        else print("No");
    }

    /// <summary>
    /// Calculates the trick winner and loser (only 1vs1 version)
    /// </summary>
    /// <returns>True if someone wins, false if draw</returns>
    private bool CalculateTrickWinner1vs1()
    {
        Card card0 = players[0].playedCard;
        Card card1 = players[1].playedCard;

        // Comprobamos el empate exacto primero. Si son idénticas en rango y palo, es empate.
        if (card0.GetCardSuit() == card1.GetCardSuit() && card0.GetCardRank() == card1.GetCardRank())
        {
            return false; // Empate
        }

        bool player0Wins = false;

        // CASO 1: Ambos juegan el palo de triunfo
        if (card0.GetCardSuit() == triumphSuit && card1.GetCardSuit() == triumphSuit)
        {
            player0Wins = card0.GetCardRank() > card1.GetCardRank();
        }
        // CASO 2: Solo el jugador 0 juega triunfo
        else if (card0.GetCardSuit() == triumphSuit)
        {
            player0Wins = true;
        }
        // CASO 3: Solo el jugador 1 juega triunfo
        else if (card1.GetCardSuit() == triumphSuit)
        {
            player0Wins = false;
        }
        // CASO 4: Ninguno juega triunfo. Ambos juegan el trickSuit (el palo que inició la baza)
        else if (card0.GetCardSuit() == trickSuit && card1.GetCardSuit() == trickSuit)
        {
             player0Wins = card0.GetCardRank() > card1.GetCardRank();
        }
        // CASO 5: Solo el jugador 0 juega el trickSuit (el jugador 1 tiró otro palo que no es triunfo)
        else if (card0.GetCardSuit() == trickSuit)
        {
            player0Wins = true;
        }
        // CASO 6: Solo el jugador 1 juega el trickSuit (el jugador 0 tiró otro palo que no es triunfo)
        else if (card1.GetCardSuit() == trickSuit)
        {
            player0Wins = false;
        }

        // Asignamos ganadores y perdedores basados en el resultado
        if (player0Wins)
        {
            trickWinner = players[0];
            trickLoser = players[1];
        }
        else
        {
            trickWinner = players[1];
            trickLoser = players[0];
        }

        return true;
    }

    public void ClearTableVisuals()
    {
        CardDisplay[] cardsOnTable = centerTableTransform.GetComponentsInChildren<CardDisplay>();
        foreach(CardDisplay card in cardsOnTable)
        {
            card.ReturnToPool();
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
        print("Next player is: " + players[currentPlayer]);
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
        print("All players have played");
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
        HandleAnimationEnded();
    }

    private void HandleAnimationEnded()
    {
        print("Animation ended");
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
            onTrickEnded.Raise();
            print("Trick ended");
        }
        if(!GameManager.Instance.isGameEnded)
            CheckEnemyTurn();
    }
    
    private void DiscoverTriumphSuit()
    {
        print("Discovering triumph suit...");
        Card.Suit newTriumphSuit = (Card.Suit)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(Card.Suit)).Length);
        foreach(Card card in triumphCards)
        {
            if(card.GetCardSuit() == newTriumphSuit)
            {
                triumphCardDisplay.sprite = card.GetCardSprite();
                break; 
            }
        }
        triumphCardDisplay.gameObject.SetActive(true);
        triumphSuit = newTriumphSuit;
        print("The triumph suit is: " + newTriumphSuit);
    }
}
