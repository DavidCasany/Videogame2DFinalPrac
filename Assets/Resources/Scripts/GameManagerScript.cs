using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    [Header("Configuració de Progrés")]
    [Tooltip("El nombre total de sales que s'han de completar per passar el joc")]
    public int totalSalesAlJoc = 3;

    private int salesCompletades = 0;
    private bool monitorDisponible = false; // Bandera per saber si es pot parlar amb el monitor

    void Awake()
    {
        // Sistema Singleton per poder accedir des de qualsevol lloc amb GameManagerScript.instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: Fa que el GameManager sobrevisqui als canvis d'escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Aquesta funció la criden automàticament les teves sales al completar-se
    public void ComprovarProgresJoc()
    {
        salesCompletades++;

        // Un cop completes qualsevol sala, es dona permís per parlar amb el monitor
        monitorDisponible = true;

        Debug.Log($"Sala completada! Progrés actual: {salesCompletades}/{totalSalesAlJoc}. Monitor disponible.");
    }

    // El monitor cridarà aquesta funció per saber si ha de respondre al jugador
    public bool PotInteractuarMonitor()
    {
        // Si ja s'han completat totes les sales, el monitor SEMPRE estarà disponible per fer el final
        if (salesCompletades >= totalSalesAlJoc)
        {
            return true;
        }

        return monitorDisponible;
    }

    // Serveix per indicar si ja estem a la fase final del joc
    public bool EstanTotesLesSalesCompletades()
    {
        return salesCompletades >= totalSalesAlJoc;
    }

    // S'executa en acabar la conversa normal per bloquejar el monitor fins a la següent sala
    public void ConsumirInteraccioMonitor()
    {
        monitorDisponible = false;
        Debug.Log("Interacció consumida. Has de completar una nova sala per rebre una altra bateria!");
    }
}