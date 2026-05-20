using UnityEngine;
using UnityEngine.SceneManagement; // Necessari per al canvi d'escena final

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

    // Variables de control intern
    private bool jugadorAProp = false;
    private bool llegintDialeg = false;
    private EnergiaControllerScript energia;

    void Start()
    {
        energia = FindObjectOfType<EnergiaControllerScript>();
    }

    void Update()
    {
        // Demanem permís al GameManager de forma constant
        if (GameManagerScript.instance != null)
        {
            bool tePermis = GameManagerScript.instance.PotInteractuarMonitor();

            // Mostrem la icona "E" només si el jugador és a prop, no està parlant i el GameManager dona permís
            if (iconaInteraccio != null)
            {
                iconaInteraccio.SetActive(jugadorAProp && tePermis && !llegintDialeg);
            }

            // Iniciem el diàleg només si el GameManager ho permet
            if (jugadorAProp && tePermis && !llegintDialeg && Input.GetKeyDown(KeyCode.E))
            {
                ComencarInteraccio();
            }
        }

        // 2. Detectem quan el diàleg s'acaba (el panel es desactiva sol pel DialegTextScript)
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

        // COMPROVACIÓ AMB EL GAME MANAGER
        if (GameManagerScript.instance != null)
        {
            // CAS A: S'han completat totes les sales del joc -> CANVI D'ESCENA
            if (GameManagerScript.instance.EstanTotesLesSalesCompletades())
            {
                Debug.Log("Joc completat! Carregant escena final...");
                SceneManager.LoadScene(indexEscenaFinal);
                return; // Sortim per evitar spawnejar la bateria
            }
            // CAS B: Encara queden sales -> Entrega de bateria normal i consumim el permís
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