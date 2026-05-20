using UnityEngine;
using UnityEngine.SceneManagement; // NOU: Necessari per al canvi d'escena

public class FinalTutorialScript : MonoBehaviour
{
    [Header("Elements Visuals i UI")]
    public GameObject iconaInteraccio;
    public GameObject panelText;

    [Header("Monitor Interactiu")]
    public Animator monitorAnimator;

    [Header("Recompensa i Escena")]
    public GameObject consumiblePrefab;
    public float yOffsetBateria = 2f; // Altura extra sobre el cap del jugador
    public int indexEscenaSeguent = 3; // Escena número 3 segons Build Settings

    // Variables de control intern
    private bool jugadorAProp = false;
    private bool llegintDialeg = false;
    private PlayerControllerScript playerScript; // Guardem la referència del jugador

    void OnEnable()
    {
        if (iconaInteraccio != null) iconaInteraccio.SetActive(true);
        if (panelText != null) panelText.SetActive(false);
        if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", true);

        llegintDialeg = false;
    }

    void Update()
    {
        // 1. Detectar quan el jugador prem la tecla E
        if (jugadorAProp && !llegintDialeg && Input.GetKeyDown(KeyCode.E))
        {
            llegintDialeg = true;

            if (iconaInteraccio != null) iconaInteraccio.SetActive(false);
            if (panelText != null) panelText.SetActive(true);
            if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", false);

            // NOU: Bloquegem els controls del jugador al començar el diàleg
            if (playerScript != null)
            {
                playerScript.DesactivarControls();
            }
        }

        // 2. Comprovar quan el panell de text es tanca sol (perquè el text ha acabat)
        if (llegintDialeg && panelText != null && !panelText.activeSelf)
        {
            llegintDialeg = false; // Evitem que aquest bloc s'executi repetidament

            // NOU: Spawnegem la bateria sobre el jugador (mateixa X, Y + offset)
            if (consumiblePrefab != null && playerScript != null)
            {
                Vector3 spawnPos = new Vector3(
                    playerScript.transform.position.x,
                    playerScript.transform.position.y + yOffsetBateria,
                    0
                );
                Instantiate(consumiblePrefab, spawnPos, Quaternion.identity);
            }

            if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", true);

            // NOU: Esperem 2 segons i canviem a l'escena 3
            Invoke("FinalitzarEscenaTutorial", 2f);
        }
    }

    private void FinalitzarEscenaTutorial()
    {
        SceneManager.LoadScene(indexEscenaSeguent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = true;
            // Obtenim la referència del script del jugador per poder moure'l o desactivar-lo
            playerScript = collision.GetComponent<PlayerControllerScript>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = false;
        }
    }
}