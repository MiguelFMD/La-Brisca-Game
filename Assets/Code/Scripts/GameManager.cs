using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }
    [SerializeField] public Player[] players;
    [SerializeField] public Deck deck;
    [SerializeField] public TrickManager trickManager;
    [SerializeField] private int cardsToDeal; 
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
        PrepareGame();
    }

    public void PrepareGame()
    {
        //Deal cards
        DealCards();
        //Discover Triumph Suit
        DiscoverTriumphSuit();
    }

    public void PlayTestTrick()
    {
        
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
}
