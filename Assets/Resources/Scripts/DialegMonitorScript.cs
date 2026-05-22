using UnityEngine;
using UnityEngine.SceneManagement; 

public class DialegMonitorScript : MonoBehaviour
{
    [Header("Elements Visuals i UI")]
    public GameObject iconaInteraccio;
    public GameObject panelText;
    public Animator monitorAnimator;

    [Header("Recompensa")]
    public GameObject consumiblePrefab;
    [Tooltip("Alçada extra sobre la posició d'aquest objecte")]
    public float yOffsetBateria = 1.5f;

    [Header("Configuració de l'Escena Final")]
    [Tooltip("Número d'escena a carregar quan es completen totes les sales")]
    public int indexEscenaFinal = 4;


    private bool jugadorAProp = false;
    private bool llegintDialeg = false;
    private EnergiaControllerScript energia;

    void Start()
    {
        energia = FindObjectOfType<EnergiaControllerScript>();
    }

    void Update()
    {
   
        if (GameManagerScript.instance != null)
        {
            bool tePermis = GameManagerScript.instance.PotInteractuarMonitor();

            if (iconaInteraccio != null)
            {
                iconaInteraccio.SetActive(jugadorAProp && tePermis && !llegintDialeg);
            }

            if (jugadorAProp && tePermis && !llegintDialeg && Input.GetKeyDown(KeyCode.E))
            {
                ComencarInteraccio();
            }
        }

        if (llegintDialeg && panelText != null && !panelText.activeSelf)
        {
            FinalitzarInteraccio();
        }
    }

    void ComencarInteraccio()
    {
        llegintDialeg = true;

        if (iconaInteraccio != null) iconaInteraccio.SetActive(false);
        if (panelText != null) panelText.SetActive(true);
        if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", false);

        if (energia != null) energia.SetPausaEnergia(true);
    }

    void FinalitzarInteraccio()
    {
        llegintDialeg = false;

        if (energia != null) energia.SetPausaEnergia(false);
        if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", true);

      
        if (GameManagerScript.instance != null)
        {
            
            if (GameManagerScript.instance.EstanTotesLesSalesCompletades())
            {
                Debug.Log("Joc completat! Carregant escena final...");
                SceneManager.LoadScene(indexEscenaFinal);
                return; 
            }
          
            else
            {
                SpawnejarBateria();
                GameManagerScript.instance.ConsumirInteraccioMonitor();
            }
        }
    }

    void SpawnejarBateria()
    {
        if (consumiblePrefab != null)
        {
            Vector3 spawnPos = new Vector3(
                transform.position.x,
                transform.position.y + yOffsetBateria,
                transform.position.z
            );

            Instantiate(consumiblePrefab, spawnPos, Quaternion.identity);
            Debug.Log("Bateria generada sobre el monitor.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = false;
            if (iconaInteraccio != null)
            {
                iconaInteraccio.SetActive(false);
            }
        }
    }
}