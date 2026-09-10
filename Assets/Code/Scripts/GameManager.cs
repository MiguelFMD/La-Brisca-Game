using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
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
}
