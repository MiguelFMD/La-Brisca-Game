using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

public class TrickManagerEditor : EditorWindow
{
    VisualElement root;
    private GameEvent onPlayerVictory;
    private GameEvent onGameOver;
    private CardGameEvent onCardAdded;
    [MenuItem("Window/UI Toolkit/TrickManagerEditor")]
    public static void ShowExample()
    {
        TrickManagerEditor wnd = GetWindow<TrickManagerEditor>();
        wnd.titleContent = new GUIContent("TrickManagerEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        
        root.Add(AddGameEventField(onPlayerVictory));
        root.Add(AddGameEventField(onGameOver));
        root.Add(AddCardEventField(onCardAdded));

    }


    private ObjectField AddGameEventField(GameEvent gameEvent)
    {
        ObjectField eventField = new ObjectField("Game Event");
        eventField.objectType = typeof(GameEvent);
        eventField.RegisterValueChangedCallback(evt =>
        {
            gameEvent = evt.newValue as GameEvent;
            Debug.Log($"Evento asignado: {(gameEvent != null ? gameEvent.name : "Ninguno")}");
            eventField.name = gameEvent.name;
            root.Add(CreateButtonGameEvent(gameEvent));
        });
        
        return eventField;
    }

    private Button CreateButtonGameEvent(GameEvent myEvent)
    {
        Button actionButton = new Button();
        actionButton.text = myEvent.name;
        actionButton.clicked += () => 
        {
            if(myEvent != null) myEvent.Raise();
            else Debug.LogWarning("First assign an event to the button");
        };
        return actionButton;
    }

    private ObjectField AddCardEventField(CardGameEvent cardGameEvent)
    {
        ObjectField eventField = new ObjectField("Card Game Event");
        eventField.objectType = typeof(CardGameEvent);
        eventField.RegisterValueChangedCallback(evt =>
        {
            cardGameEvent = evt.newValue as CardGameEvent;
            Debug.Log($"Evento asignado: {(cardGameEvent != null ? cardGameEvent.name : "Ninguno")}");
            CreateButtonCardEvent(cardGameEvent, Suit.Clubs);
            CreateButtonCardEvent(cardGameEvent, Suit.Cups);
            CreateButtonCardEvent(cardGameEvent, Suit.Golds);
            CreateButtonCardEvent(cardGameEvent, Suit.Swords);
        });
        return eventField;
    }

    private Button CreateButtonCardEvent(CardGameEvent myEvent, Suit suit)
    {
        CardData cardData = ScriptableObject.CreateInstance<CardData>();
        cardData.Initialize(suit, Rank.Ace);
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        CardStatus cardStatus = new CardStatus(cardData, player.state);
        Button actionButton = new Button();
        actionButton.text = suit.ToString();
        actionButton.clicked += () => 
        {
            if(myEvent != null) myEvent.Raise(cardStatus);
            else Debug.LogWarning("First assign an event to the button");
        };
        return actionButton;
    }
}
