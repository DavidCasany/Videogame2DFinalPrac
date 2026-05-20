using UnityEngine;
using System.Collections;

public class PlayerRespawnScript : MonoBehaviour
{
    [Header("Configuració Respawn")]
    public float tempsEsperaRespawn = 0.5f;

    public RoomManagerScript salaActual;

    private Vector3 ultimCheckpoint;
    private Rigidbody2D rb;
    public bool estaMort = false;

    void Start()
    {
        ultimCheckpoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (estaMort) return; // Si està morint, ignorem els inputs

        // Reinici manual ràpid
        if (Input.GetKeyDown(KeyCode.R))
        {
            TornarAlCheckpointImmediat();
        }
    }

    public void SetNewCheckpoint(Vector3 posicio)
    {
        ultimCheckpoint = posicio;
    }

    // ----------------------------------------------------
    // DETECCIÓ D'OBSTACLES (Mort)
    // ----------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && !estaMort)
        {
            StartCoroutine(ProcesMort());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !estaMort)
        {
            StartCoroutine(ProcesMort());
        }
    }

    // ----------------------------------------------------
    // PROCÉS DE MORT I RESPAWN
    // ----------------------------------------------------

    IEnumerator ProcesMort()
    {
        estaMort = true;
        rb.velocity = Vector2.zero;
        rb.simulated = false; // Congelem el jugador per a que no caigui més

        // NOU: Avisem al controlador que posi l'animació de Ferit
        PlayerControllerScript playerCtrl = GetComponent<PlayerControllerScript>();
        if (playerCtrl != null)
        {
            playerCtrl.ActivarEstatFerit(true);
        }

        // Esperem el temps marcat a l'inspector
        yield return new WaitForSeconds(tempsEsperaRespawn);

        // 1. REINICIEM LA SALA
        if (salaActual != null)
        {
            salaActual.ReiniciarSala();
        }

        // 2. Tornem al punt de control
        transform.position = ultimCheckpoint;

        // 3. Descongelem el jugador
        rb.simulated = true;
        estaMort = false;

        // 4. Apaguem l'animació de Ferit i resetejem salts
        if (playerCtrl != null)
        {
            playerCtrl.ActivarEstatFerit(false);
            playerCtrl.ResetSaltsIAbilitat();
        }
    }

    public void TornarAlCheckpointImmediat()
    {
        StopAllCoroutines();
        estaMort = false;
        rb.simulated = true;
        transform.position = ultimCheckpoint;
        rb.velocity = Vector2.zero;

        // Assegurem-nos de treure l'animació de ferit si reiniciem manualment a mig procés
        PlayerControllerScript playerCtrl = GetComponent<PlayerControllerScript>();
        if (playerCtrl != null)
        {
            playerCtrl.ActivarEstatFerit(false);
            playerCtrl.ResetSaltsIAbilitat();
        }

        if (salaActual != null)
        {
            salaActual.ReiniciarSala();
        }
    }
}