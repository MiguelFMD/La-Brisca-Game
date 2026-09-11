using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }
    [SerializeField] public Player[] players;
    [SerializeField] public Deck deck;
    [SerializeField] public TrickManager trickManager;
    [SerializeField] private int cardsToDeal;
    private int currentPlayer;
    private bool isFirstTrickPlay = true;

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

    public void PrepareGame()
    {
        //Reset deck
        deck.CreateDeck();
        //Clear player hands
        foreach(Player player in players)
            player.ClearHand();
        //Deal cards
        DealCards();
        //Discover Triumph Suit
        DiscoverTriumphSuit();
        //Select random player to start
        currentPlayer = Random.Range(0, players.Length - 1);
        isFirstTrickPlay = true;
        print("Game prepared");
    }

    public void PlayTestTrick()
    {
        print("Player " + currentPlayer + " is playing");
        players[currentPlayer].PlayRandomCard();
        if(isFirstTrickPlay)
            trickManager.SetTrickSuit(players[currentPlayer].playedCard.cardSuit);
        SelectNextPlayer();
        if(CheckAllPlayersHavePlayed())
        {
            Player winner = trickManager.CalculateTrickWinner();
            print("The player winner is: " + winner);
            RemovePlayerHands();
            isFirstTrickPlay = true;
        }
    }

    private void DealCards()
    {
        print("Dealing cards...");
        for(int c = 0; c < cardsToDeal; c++)
        {
            print("Card number: " + c);
            foreach(Player player in players)
            {
                Card card = deck.RemoveCard();
                if(card)
                {
                    player.DrawCard(card);
                }
            }
        }
    }

    private void DiscoverTriumphSuit()
    {
        print("Discovering triumph suit...");
        Card card = deck.RemoveCard();
        trickManager.SetTriumphSuit(card.cardSuit);
        print("The triumph suit is: " + card.cardSuit);
    }

    private void SelectNextPlayer()
    {
        if(currentPlayer >= players.Length)
            currentPlayer = 0;
        else
            currentPlayer++;
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

    private void RemovePlayerHands()
    {
        print("Removing played cards from players...");
        foreach(Player player in players)
        {
            player.ClearPlayedCard();
        }
    }
}
