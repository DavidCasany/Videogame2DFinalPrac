using UnityEngine;

public class FinalTutorialScript : MonoBehaviour
{
    [Header("Elements Visuals i UI")]
    public GameObject iconaInteraccio;
    public GameObject panelText;

    [Header("Monitor Interactiu")]
    public Animator monitorAnimator; // NOU: Referència a l'animador del monitor

    [Header("Recompensa")]
    public GameObject consumiblePrefab;

    // Variables de control intern
    private bool jugadorAProp = false;
    private bool llegintDialeg = false;

    // S'executa cada vegada que s'activa aquest objecte
    void OnEnable()
    {
        // Ens assegurem que la icona es vegi i el text estigui amagat d'inici
        if (iconaInteraccio != null) iconaInteraccio.SetActive(true);
        if (panelText != null) panelText.SetActive(false);

        // NOU: Ens assegurem que el monitor comenci amb la pantalla vermella (ScreenRed = true)
        if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", true);

        llegintDialeg = false;
    }

    void Update()
    {
        // 1. Detectar quan el jugador prem la tecla E per començar
        if (jugadorAProp && !llegintDialeg && Input.GetKeyDown(KeyCode.E))
        {
            llegintDialeg = true;

            // Amaguem la icona i mostrem el panel
            if (iconaInteraccio != null) iconaInteraccio.SetActive(false);
            if (panelText != null) panelText.SetActive(true);

            // NOU: Canviem l'estat del monitor a "no vermell" (verd/normal)
            if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", false);
        }

        // 2. Comprovar si estem enmig d'un diàleg i el panell s'ha tancat sol
        if (llegintDialeg && panelText != null && !panelText.activeSelf)
        {
            // El DialegTextScript ha desactivat el panel, per tant el text ha acabat!

            // Spawnegem el consumible just en la posició d'aquest objecte
            if (consumiblePrefab != null)
            {
                Instantiate(consumiblePrefab, transform.position, Quaternion.identity);
            }

            // NOU: Retornem l'animació del monitor al seu estat original
            if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", true);

            // Desactivem l'objecte FinalTutorial per completar-ho tot
            gameObject.SetActive(false);
        }
    }

    // Gestionem quan el jugador entra al trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = true;
        }
    }

    // Gestionem quan el jugador surt del trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = false;
        }
    }
}