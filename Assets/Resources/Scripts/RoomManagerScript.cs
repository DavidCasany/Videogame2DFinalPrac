using UnityEngine;

public class RoomManagerScript : MonoBehaviour
{
    [Header("Configuració de la Sala")]
    public string nomDeLaSala = "Sala 1";
    public Transform puntDeSpawn;
    public Transform puntDeCamara;

    [Header("Portes i Sortides")]
    public GameObject[] portesDeLaSala; 
    public bool tancarEnEntrar = true;

    [Header("Objectes Activables")]
    public GameObject[] objectesDeLaSala; 
    
    // Variables d'estat
    private bool salaCompletada = false; // Només es fa true al SORTIR de la sala sa i estalvi
    private bool jugadorDins = false;
    private bool portesObertesActualment = false; 

    private Camera camaraPrincipal;

    void Start()
    {
        camaraPrincipal = Camera.main;
        
        // Inicialment les portes estan obertes fins que el jugador hi entra
        SetPortesEstat(false);
    }

    void Update()
    {
        if (salaCompletada) return;

        // Si el jugador és dins, mirem si ha agafat tots els objectes
        if (jugadorDins)
        {
            if (EstanTotsAgafats() && !portesObertesActualment)
            {
                // OBRIM LES PORTES per poder passar, però encara NO completem la sala definitivament
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

            // 1. Movem la càmera
            if (puntDeCamara != null && camaraPrincipal != null)
                camaraPrincipal.transform.position = new Vector3(puntDeCamara.position.x, puntDeCamara.position.y, -10f);

            // 2. Registrem la sala al jugador i actualitzem el Respawn
            PlayerRespawnScript respawnScript = other.GetComponent<PlayerRespawnScript>();
            if (respawnScript != null)
            {
                respawnScript.salaActual = this;
                if (puntDeSpawn != null) respawnScript.SetNewCheckpoint(puntDeSpawn.position);
            }

            // 3. Tancar portes NOMÉS si la sala NO està completada
            if (tancarEnEntrar && !salaCompletada)
            {
                SetPortesEstat(true);
                portesObertesActualment = false;
                Debug.Log("Portes tancades. Atrapa els objectes!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDins = false;

            // Busquem l'script del jugador per veure el seu estat
            PlayerRespawnScript respawnScript = other.GetComponent<PlayerRespawnScript>();
            
            // SI ESTÀ MORT (per haver tocat punxes i quedar-se congelat), IGNOREM LA SORTIDA
            if (respawnScript != null && respawnScript.estaMort)
            {
                return; 
            }

            // Si està viu, ha sortit caminant per la porta. Verifiquem els objectes:
            if (!salaCompletada && EstanTotsAgafats())
            {
                CompletarSalaDefinitivament();
            }
        }
    }

    private void CompletarSalaDefinitivament()
    {
        salaCompletada = true;
        Debug.Log("<color=green>SALA COMPLETADA DEFINITIVAMENT: </color>" + nomDeLaSala);

        // Ens assegurem que les portes queden obertes per sempre
        SetPortesEstat(false);

        // AVISAR AL GAME MANAGER QUE HEM FET UNA SALA MÉS
        if (GameManagerScript.instance != null)
        {
            GameManagerScript.instance.ComprovarProgresJoc();
        }
    }

    // --- FUNCIÓ DE REINICI DE LA SALA ---
    public void ReiniciarSala()
    {
        // Si la sala ja s'havia guardat al sortir, no la toquem. Reapareixeràs aquí però tot seguirà obert.
        if (salaCompletada) 
        {
            return;
        }

        // Si el jugador mor abans de sortir, reiniciem ELS OBJECTES
        if (objectesDeLaSala != null)
        {
            foreach (GameObject obj in objectesDeLaSala)
            {
                if (obj != null) obj.SetActive(true);
            }
        }

        // Tornem a tancar les portes per al nou intent
        if (tancarEnEntrar)
        {
            SetPortesEstat(true);
            portesObertesActualment = false;
        }
        
        Debug.Log("Sala " + nomDeLaSala + " reiniciada!");
    }

    // Funció auxiliar per canviar totes les portes de cop
    private void SetPortesEstat(bool estat)
    {
        if (portesDeLaSala != null)
        {
            foreach (GameObject porta in portesDeLaSala)
            {
                if (porta != null) porta.SetActive(estat);
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

    // Permet al Game Manager llegir si la sala està completada
    public bool GetSalaCompletada() 
    {
        return salaCompletada;
    }
}