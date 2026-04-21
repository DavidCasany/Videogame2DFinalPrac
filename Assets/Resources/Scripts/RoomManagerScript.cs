using UnityEngine;

public class RoomManagerScript : MonoBehaviour
{
    [Header("Configuració de Sala")]
    public string nomDeLaSala = "Sala 1";
    public Transform puntDeSpawn;
    public Transform puntDeCamara;

    [Header("Control de Portes")]
    public GameObject objectePorta; // L'objecte que bloqueja la sortida
    public bool tancarEnEntrar = true;

    [Header("Objectes Activables")]
    public GameObject[] objectesDeLaSala; 
    private bool salaCompletada = false;
    private bool jugadorDins = false;

    private Camera camaraPrincipal;

    void Start()
    {
        camaraPrincipal = Camera.main;
        
        // Al principi del joc, la porta hauria d'estar oberta (desactivada)
        // o tancada segons com dissenyis el nivell.
        if (objectePorta != null && tancarEnEntrar) 
        {
            objectePorta.SetActive(false); 
        }
    }

    void Update()
    {
        if (salaCompletada) return;

        // Només comprovem si s'ha completat si el jugador ja és dins
        if (jugadorDins && EstanTotsAgafats())
        {
            CompletarSala();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDins = true;

            // 1. Posem la càmera al seu lloc
            if (puntDeCamara != null)
                camaraPrincipal.transform.position = new Vector3(puntDeCamara.position.x, puntDeCamara.position.y, -10f);

            // 2. Actualitzem el Respawn
            PlayerRespawnScript respawnScript = other.GetComponent<PlayerRespawnScript>();
            if (respawnScript != null && puntDeSpawn != null)
                respawnScript.SetNewCheckpoint(puntDeSpawn.position);

            // 3. BLOQUEJEM LA SALA
            if (objectePorta != null && tancarEnEntrar && !salaCompletada)
            {
                objectePorta.SetActive(true);
                Debug.Log("Porta tancada! Completa la sala per sortir.");
            }
        }
    }

    private void CompletarSala()
    {
        salaCompletada = true;
        Debug.Log("<color=green>SALA COMPLETADA: </color>" + nomDeLaSala);

        // OBRIM LA PORTA
        if (objectePorta != null)
        {
            objectePorta.SetActive(false);
            Debug.Log("Porta oberta! Pots continuar.");
        }
    }

    bool EstanTotsAgafats()
    {
        if (objectesDeLaSala.Length == 0) return true; 

        foreach (GameObject obj in objectesDeLaSala)
        {
            if (obj != null && obj.activeSelf) return false; 
        }
        return true;
    }
    

}
