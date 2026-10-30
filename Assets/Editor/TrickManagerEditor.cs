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

        
        root.Add(FieldEvent(onPlayerVictory));
        root.Add(FieldEvent(onGameOver));

        // 2. Crear un Dropdown (Menú desplegable)
        /*List<string> trickOptions = new List<string> { "Salto", "Giro", "Aceleración" };
        
        // Parámetros: Etiqueta, Lista de opciones, Índice por defecto (0)
        DropdownField trickDropdown = new DropdownField("Selecciona un truco", trickOptions, 0);
        
        // Suscribirse al evento de cambio de valor
        trickDropdown.RegisterValueChangedCallback(evt =>
        {
            Debug.Log($"Nueva opción seleccionada: {evt.newValue}. Llama a tu evento aquí.");
            // Ejemplo: MiGameManager.Instancia.PrepararTruco(evt.newValue);
        });
        root.Add(trickDropdown);*/

    }

    private ObjectField FieldEvent(GameEvent gameEvent)
    {
        ObjectField eventField = new ObjectField("Game Event");
        eventField.objectType = typeof(GameEvent);
        eventField.RegisterValueChangedCallback(evt =>
        {
            gameEvent = evt.newValue as GameEvent;
            Debug.Log($"Evento asignado: {(gameEvent != null ? gameEvent.name : "Ninguno")}");
            eventField.name = gameEvent.name;
            root.Add(CreateButton(gameEvent));
        });
        
        return eventField;
    }

    private Button CreateButton(GameEvent myEvent)
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
}
