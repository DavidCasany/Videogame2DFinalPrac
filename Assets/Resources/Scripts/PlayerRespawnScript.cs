using UnityEngine;
using System.Collections; // Necessari per utilitzar IEnumerator

public class PlayerRespawnScript : MonoBehaviour
{
    [Header("Configuració Respawn")]
    public float tempsEsperaRespawn = 0.5f;

    // AQUESTA ÉS LA VARIABLE QUE FALTAVA (La posem HideInInspector perquè s'omple sola i no cal veure-la)
    [HideInInspector] public RoomManagerScript salaActual; 

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

    // Aquesta funció la crida la sala per guardar on hem de reaparèixer
    public void SetNewCheckpoint(Vector3 posicio)
    {
        ultimCheckpoint = posicio;
    }

    // ----------------------------------------------------
    // DETECCIÓ D'OBSTACLES (Mort)
    // ----------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Assegura't que les punxes / foc tinguin el Tag "Obstacle"
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

        // Esperem el temps marcat a l'inspector
        yield return new WaitForSeconds(tempsEsperaRespawn);

        // 1. REINICIEM LA SALA (Activa els Floppy Disks i tanca portes)
        if (salaActual != null)
        {
            salaActual.ReiniciarSala();
        }

        // 2. Tornem al punt de control
        transform.position = ultimCheckpoint;
        
        // 3. Descongelem el jugador
        rb.simulated = true;
        estaMort = false;

        // 4. Resetejem l'habilitat W i el doble salt
        GetComponent<PlayerControllerScript>().ResetSaltsIAbilitat();
    }

    public void TornarAlCheckpointImmediat()
    {
        StopAllCoroutines(); // Aturem la mort si havíem tocat un obstacle
        estaMort = false;
        rb.simulated = true;
        transform.position = ultimCheckpoint;
        rb.velocity = Vector2.zero;
        
        if (salaActual != null)
        {
            salaActual.ReiniciarSala();
        }

        GetComponent<PlayerControllerScript>().ResetSaltsIAbilitat();
    }
}