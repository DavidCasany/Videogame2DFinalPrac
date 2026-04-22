using UnityEngine;

public class RoomManagerScript : MonoBehaviour
{
    [Header("Configuració de Sala")]
    public string nomDeLaSala = "Sala 1";
    public Transform puntDeSpawn;
    public Transform puntDeCamara;

    [Header("Control de Portes")]
    // ARA ÉS UNA LLISTA
    public GameObject[] portesDeLaSala; 
    public bool tancarEnEntrar = true;

    [Header("Objectes Activables")]
    public GameObject[] objectesDeLaSala; 
    private bool salaCompletada = false;
    private bool jugadorDins = false;

    private Camera camaraPrincipal;

    void Start()
    {
        camaraPrincipal = Camera.main;
        
        if (portesDeLaSala != null && tancarEnEntrar) 
        {
            foreach (GameObject porta in portesDeLaSala)
            {
                if (porta != null) porta.SetActive(false); 
            }
        }
    }

    void Update()
    {
        if (salaCompletada) return;

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

            // Càmera
            if (puntDeCamara != null)
                camaraPrincipal.transform.position = new Vector3(puntDeCamara.position.x, puntDeCamara.position.y, -10f);

            // ACTUALITZEM LA SALA ACTUAL I EL CHECKPOINT
            PlayerRespawnScript respawnScript = other.GetComponent<PlayerRespawnScript>();
            if (respawnScript != null)
            {
                respawnScript.salaActual = this; // Aquí és on donava l'error
                if (puntDeSpawn != null) respawnScript.SetNewCheckpoint(puntDeSpawn.position);
            }

            // Tancamos les portes
            if (portesDeLaSala != null && tancarEnEntrar && !salaCompletada)
            {
                foreach (GameObject porta in portesDeLaSala)
                {
                    if (porta != null) porta.SetActive(true);
                }
            }
        }
    }

    public void ReiniciarObjectes()
    {
        salaCompletada = false;
        foreach (GameObject obj in objectesDeLaSala)
        {
            if (obj != null) obj.SetActive(true);
        }
        
        // Opcional: tornar a tancar portes si havien quedat obertes
        if (portesDeLaSala != null && tancarEnEntrar)
        {
            foreach (GameObject porta in portesDeLaSala)
            {
                if (porta != null) porta.SetActive(true);
            }
        }
    }

    private void CompletarSala()
    {
        salaCompletada = true;
        if (portesDeLaSala != null)
        {
            foreach (GameObject porta in portesDeLaSala)
            {
                if (porta != null) porta.SetActive(false);
            }
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