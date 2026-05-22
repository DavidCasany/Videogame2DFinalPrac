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
        if (estaMort) return; 
        if (Input.GetKeyDown(KeyCode.R))
        {
            TornarAlCheckpointImmediat();
        }
    }

    public void SetNewCheckpoint(Vector3 posicio)
    {
        ultimCheckpoint = posicio;
    }


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

   
    IEnumerator ProcesMort()
    {
        estaMort = true;
        rb.velocity = Vector2.zero;
        rb.simulated = false; 

        PlayerControllerScript playerCtrl = GetComponent<PlayerControllerScript>();
        if (playerCtrl != null)
        {
            playerCtrl.ActivarEstatFerit(true);
        }

      
        yield return new WaitForSeconds(tempsEsperaRespawn);

       
        if (salaActual != null)
        {
            salaActual.ReiniciarSala();
        }

        
        transform.position = ultimCheckpoint;


        rb.simulated = true;
        estaMort = false;

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