using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    [Header("Configuració de Final de Joc")]
    public int escenaFinalIndex = 3; // L'índex de la nova escena (ex: Crèdits o Victòria)
    
    private RoomManagerScript[] totesLesSales;

    void Awake()
    {
        // Sistema Singleton per poder accedir des de qualsevol lloc
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Busquem automàticament totes les sales que hi ha a l'escena
        totesLesSales = FindObjectsOfType<RoomManagerScript>();
        Debug.Log("S'han trobat " + totesLesSales.Length + " sales al nivell.");
    }

    // Aquesta funció la cridaran les sales cada cop que es completin
    public void ComprovarProgresJoc()
    {
        int salesCompletades = 0;

        foreach (RoomManagerScript sala in totesLesSales)
        {
            // Accedim a la variable salaCompletada del script de la sala
            // Nota: Hauràs de posar la variable 'salaCompletada' com a 'public' al RoomManager
            if (sala.GetSalaCompletada()) 
            {
                salesCompletades++;
            }
        }

        Debug.Log("Progrés: " + salesCompletades + "/" + totesLesSales.Length);

        if (salesCompletades >= totesLesSales.Length)
        {
            FinalitzarJoc();
        }
    }

    void FinalitzarJoc()
    {
        Debug.Log("<color=cyan>FELICITATS! TOTES LES SALES COMPLETADES.</color>");
        SceneManager.LoadScene(escenaFinalIndex);
    }
}