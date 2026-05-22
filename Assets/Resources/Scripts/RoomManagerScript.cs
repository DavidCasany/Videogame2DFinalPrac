using UnityEngine;


public class RoomManagerScript : MonoBehaviour
{
    [Header("Configuració de la Sala")]
    public string nomDeLaSala = "Sala 1";
    public Transform puntDeSpawn;
    public Transform puntDeCamara;
    public bool esSalaBoss = false;

    [Header("Portes i Sortides")]
    public GameObject[] portesDeLaSala;
    public bool tancarEnEntrar = true;

    [Header("Objectes Activables")]
    public GameObject[] objectesDeLaSala;

    private bool salaCompletada = false;
    private bool jugadorDins = false;
    private bool portesObertesActualment = false;
    private Camera camaraPrincipal;

    void Start()
    {
        camaraPrincipal = Camera.main;
        SetPortesEstat(false);
    }

    void Update()
    {
        if (salaCompletada) return;

        if (jugadorDins)
        {
            if (EstanTotsAgafats() && !portesObertesActualment)
            {
                SetPortesEstat(false);
                portesObertesActualment = true;
                Debug.Log("Objectes recollits! Portes obertes, surt de la sala per confirmar.");
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
                respawnScript.salaActual = this;
                if (puntDeSpawn != null) respawnScript.SetNewCheckpoint(puntDeSpawn.position);
            }

            if (tancarEnEntrar && !salaCompletada)
            {
                SetPortesEstat(true);
                portesObertesActualment = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDins = false;
            PlayerRespawnScript respawnScript = other.GetComponent<PlayerRespawnScript>();

            if (respawnScript != null && respawnScript.estaMort)
            {
                return;
            }

            if (!salaCompletada && EstanTotsAgafats())
            {
                CompletarSalaDefinitivament();
            }
        }
    }

    private void CompletarSalaDefinitivament()
    {
        salaCompletada = true;
        SetPortesEstat(false);

        if (GameManagerScript.instance != null)
        {
            GameManagerScript.instance.ComprovarProgresJoc();
        }
    }


    public void ReiniciarSala()
    {
        if (salaCompletada || esSalaBoss)
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

        if (tancarEnEntrar)
        {
            SetPortesEstat(true);
            portesObertesActualment = false;
        }
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