using UnityEngine;
using TMPro;

public class ConsoleToUI : MonoBehaviour
{
    private TextMeshProUGUI uiText; 
    private string logHistory = "";

    void Awake()
    {
        uiText = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        // Nos suscribimos al evento cuando el script se activa
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        // Es muy importante desuscribirse cuando el script se desactiva para evitar errores de memoria
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string colorTag = "<color=white>";

        // Cambiamos el color del texto según el tipo de mensaje
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
            case LogType.Assert:
                colorTag = "<color=red>";
                break;
            case LogType.Warning:
                colorTag = "<color=yellow>";
                break;
        }

        // Añadimos el nuevo log al historial con su color y un salto de línea
        logHistory += $"{colorTag}{logString}</color>\n";

        // (Opcional) Limitar el tamaño para que no consuma demasiada memoria si hay muchos logs
        if (logHistory.Length > 5000)
        {
            logHistory = logHistory.Substring(logHistory.Length - 4000);
        }

        // Actualizamos el texto de la UI
        if (uiText != null)
        {
            uiText.text = logHistory;
        }
    }
}