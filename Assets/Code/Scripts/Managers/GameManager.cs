using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Player player;
    public Player enemy;
    public Player[] players;
    public TrickManager trickManager;
    [SerializeField] private int initialCardsAmount;

    //Card prefab and pool
    [SerializeField] private GameObject cardDisplayPrefab;
    private Queue<CardDisplay> cardPool = new Queue<CardDisplay>();
    [SerializeField] private RectTransform cardPoolTransform;
    
    
    void OnEnable()
    {
        EventManager.OnTrickEnded += HandleTrickEnded;
        //Two players mode
        player.OnPlayerDead += HandlePlayerDead;
        enemy.OnPlayerDead += HandleEnemyDead;
        players = new Player[2] {player, enemy};
    }

    void OnDisable()
    {
        EventManager.OnTrickEnded -= HandleTrickEnded;
        player.OnPlayerDead -= HandlePlayerDead;
        enemy.OnPlayerDead -= HandleEnemyDead;
    }

    private void Awake()
    {
        // 1. Verificar si ya existe una instancia
        if (Instance != null && Instance != this)
        {
            // Si ya existe otra, destruir este duplicado
            Destroy(gameObject);
            return;
        }

        // 2. Asignar la instancia actual
        Instance = this;

        // 3. Hacer que persista entre cambios de escena
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
       ResetGame();
    }

    public void ResetGame()
    {
        ResetPlayers();
        DrawCards(initialCardsAmount);
        trickManager.DiscoverTriumphSuit();
        trickManager.ResetTrick();
    }

    public CardDisplay GetCardVisual()
    {
        if (cardPool.Count > 0)
        {
            CardDisplay pooledCard = cardPool.Dequeue();
            pooledCard.gameObject.SetActive(true);
            return pooledCard;
        }
        else
        {
            //print("no habian cartas");
            // Si no hay cartas reciclables, creamos una nueva
            GameObject newCard = Instantiate(cardDisplayPrefab);
            return newCard.GetComponent<CardDisplay>();
        }
    }

    public void ReturnCardToPool(CardDisplay cardToReturn)
    {
        cardToReturn.gameObject.SetActive(false); // Hide the card
        cardPool.Enqueue(cardToReturn); // Save it for future
        cardToReturn.transform.SetParent(cardPoolTransform);
    }

    public void PlayCard(CardDisplay cardDisplay)
    {
        //print("Card clicked is: " + cardDisplay.GetCardData().GetCardRank() + " of " + cardDisplay.GetCardData().GetCardSuit());
        trickManager.CardPlayed(cardDisplay);
    }

    private void HandleTrickEnded()
    {
        /*if(deck.IsDeckEmpty() && EmptyHands()) //If the deck is empty that means the game has ended
        {
            GetWinner();
        }*/
        
        //else
        //{
            RemovePlayersPlayedCard(); //Remove the played cards from players
            DrawOneCard();
        //}
    }

    private void DrawCards(int cardsToDeal)
    {
        for(int c = 0; c < cardsToDeal; c++)
        {
            DrawOneCard();
        }
    }

    private void DrawOneCard()
    {
        foreach(Player player in players)
        {
            player.DrawCard();
        }
    }

    private void RemovePlayersPlayedCard()
    {
        //print("Removing played cards from players...");
        foreach(Player player in players)
        {
            player.ClearPlayedCard();
        }
    }

    private bool EmptyHands()
    {
        foreach(Player player in players)
        {
            if(player.IsHandEmpty())
                return true;

        }
        return false;
    }

    private void ResetPlayers()
    {
        for(int p = 0; p < players.Length; p++)
        {
            players[p].ClearHand();
            players[p].playerNumber = p + 1;
            players[p].scoredCards.Clear();
            players[p].Heal(players[p].maxHealth);
            players[p].CreateDeck();
        }
    }

    private Player GetWinner()
    {
        Player winner = players[0];
        int winnerScore = players[0].CalculateScore();
        //print("Player " + winner.playerNumber + " scored " + winnerScore);
        for(int p = 1; p < players.Length; p++)
        {
            int otherScore = players[p].CalculateScore();
            if(winnerScore < otherScore)
            {
                winnerScore = otherScore;
                winner = players[p];
            }
            //print("Player " + players[p].playerNumber + " scored " + otherScore);
        }
        print("The winner is: " + winner.name);
        return winner;
    }

    private void HandlePlayerDead()
    {
        print("YOU LOSE!");
    }

    private void HandleEnemyDead()
    {
        print("YOU WIN!");
    }

    
}
