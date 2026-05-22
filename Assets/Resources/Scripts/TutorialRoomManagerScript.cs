using UnityEngine;

public class TutorialRoomManagerScript : MonoBehaviour
{
    [Header("Configuració de la Sala")]
    public string nomDeLaSala = "Tutorial";
    public Transform puntDeSpawn;
    public Transform puntDeCamara;

    [Header("Portes i Sortides")]
    public GameObject[] portesDeLaSala;
    public bool tancarEnEntrar = true;

    [Header("Objectes Activables")]
    public GameObject[] objectesDeLaSala;

    [Header("Elements Específics del Tutorial")]
    public GameObject finalTutorial;

    // Variables d'estat
    private bool salaCompletada = false;
    private bool jugadorDins = false;
    private bool eventTutorialActivat = false;

    private Camera camaraPrincipal;

    void Start()
    {
        camaraPrincipal = Camera.main;
        SetPortesEstat(false);

        if (finalTutorial != null)
        {
            finalTutorial.SetActive(false);
        }
    }

    void Update()
    {

        if (eventTutorialActivat && finalTutorial != null && !finalTutorial.activeSelf)
        {
            SetPortesEstat(false);
            eventTutorialActivat = false; 
            Debug.Log("Tutorial finalitzat al 100%! S'obren les barreres.");
        }

        
        if (!salaCompletada && jugadorDins)
        {
            if (EstanTotsAgafats())
            {
            
                salaCompletada = true;
                CompletarSalaDefinitivament();

             
                if (finalTutorial != null)
                {
                    finalTutorial.SetActive(true);
                    eventTutorialActivat = true;
                }

                Debug.Log("Tots els objectes recollits! Sala guardada. Parla amb el NPC.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDins = true;

            if (puntDeCamara != null && camaraPrincipal != null)
                camaraPrincipal.transform.position = new Vector3(puntDeCamara.position.x, puntDeCamara.position.y, -10f);

            PlayerRespawnScript respawnScript = other.GetComponent<PlayerRespawnScript>();
            if (respawnScript != null)
            {
 
                respawnScript.salaActual = this.GetComponent<RoomManagerScript>();
                if (puntDeSpawn != null) respawnScript.SetNewCheckpoint(puntDeSpawn.position);
            }

            if (tancarEnEntrar && !salaCompletada)
            {
                SetPortesEstat(true);
                eventTutorialActivat = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDins = false;

        
        }
    }

    private void CompletarSalaDefinitivament()
    {
        Debug.Log("<color=green>TUTORIAL COMPLETAT: </color>" + nomDeLaSala);

        if (GameManagerScript.instance != null)
        {
            GameManagerScript.instance.ComprovarProgresJoc();
        }
    }

    public void ReiniciarSala()
    {

        if (salaCompletada)
        {
            return;
        }

        if (objectesDeLaSala != null)
        {
            foreach (GameObject obj in objectesDeLaSala)
            {
                if (obj != null) obj.SetActive(true);
            }
        }

        if (finalTutorial != null)
        {
            finalTutorial.SetActive(false);
        }

        if (tancarEnEntrar)
        {
            SetPortesEstat(true);
            eventTutorialActivat = false;
        }

        Debug.Log("Sala " + nomDeLaSala + " reiniciada!");
    }

    private void SetPortesEstat(bool estat)
    {
        if (portesDeLaSala != null)
        {
            foreach (GameObject porta in portesDeLaSala)
            {
                if (porta != null)
                {
                    Animator ar = porta.GetComponent<Animator>();
                    if (ar != null) ar.SetBool("BarrierState", estat);

                    Collider2D col = porta.GetComponent<Collider2D>();
                    if (col != null) col.enabled = estat;
                }
            }
        }
    }

    bool EstanTotsAgafats()
    {
        if (objectesDeLaSala == null || objectesDeLaSala.Length == 0) return true;

        foreach (GameObject obj in objectesDeLaSala)
        {
            if (obj != null && obj.activeSelf) return false;
        }
        return true;
    }

    public bool GetSalaCompletada()
    {
        return salaCompletada;
    }
}