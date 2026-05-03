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
        // 1. VIGILEM EL FINAL DEL TUTORIAL: Si l'esdeveniment està actiu però l'objecte
        // s'ha desactivat (perquè has agafat el consumible), obrim les barreres!
        if (eventTutorialActivat && finalTutorial != null && !finalTutorial.activeSelf)
        {
            SetPortesEstat(false);
            eventTutorialActivat = false; // Ho posem a fals perquè no ho repeteixi cada frame
            Debug.Log("Tutorial finalitzat al 100%! S'obren les barreres.");
        }

        // 2. VIGILEM ELS OBJECTES: Només comprovem si la sala no està completada encara
        if (!salaCompletada && jugadorDins)
        {
            if (EstanTotsAgafats())
            {
                // AQUÍ ESTÀ LA MÀGIA: Completem la sala definitivament en el mateix
                // instant en què agafem l'últim objecte.
                salaCompletada = true;
                CompletarSalaDefinitivament();

                // Activem l'objecte del tutorial final
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
                // Avís: Si el teu PlayerRespawn demana estrictament un RoomManagerScript,
                // potser has de canviar "MonoBehaviour" a dalt per "RoomManagerScript"
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

            // Com que ara hem mogut la comprovació de "completar sala" a l'Update,
            // aquí ja no cal que hi fem absolutament res en sortir! El codi queda molt més net.
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
        // Com que la sala ara es completa a l'agafar els objectes, 
        // aquest return salvarà el progrés i no els reactivarà!
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