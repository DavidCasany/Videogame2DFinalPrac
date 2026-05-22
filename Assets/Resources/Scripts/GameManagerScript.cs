using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    [Header("Configuració de Progrés")]
    public int totalSalesAlJoc = 3;

    private int salesCompletades = 0;
    private bool monitorDisponible = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void ComprovarProgresJoc()
    {
        salesCompletades++;
        monitorDisponible = true;
    }

    public bool PotInteractuarMonitor()
    {
        if (salesCompletades >= totalSalesAlJoc)
        {
            return true;
        }

        return monitorDisponible;
    }

    public bool EstanTotesLesSalesCompletades()
    {
        return salesCompletades >= totalSalesAlJoc;
    }

    public void ConsumirInteraccioMonitor()
    {
        monitorDisponible = false;
    }

    public void CarregarEscenaNeta(string nomDeLEscena)
    {
        AudioSource[] totesLesMusiques = FindObjectsOfType<AudioSource>();
        foreach (AudioSource musica in totesLesMusiques)
        {
            if (musica.isPlaying)
            {
                musica.Stop();
            }
        }

        SceneManager.LoadScene(nomDeLEscena);
    }
}