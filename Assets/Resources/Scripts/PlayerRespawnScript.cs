using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnScript : MonoBehaviour
{

    private Vector3 ultimCheckpoint;
    private Rigidbody2D rb;
    private bool estaMort = false;

    [Header("Configuració Mort")]
    public float tempsEsperaRespawn = 0.5f;

    void Start()
    {
        ultimCheckpoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (estaMort) return; // Si estem en el procés de mort, no fem res

        if (Input.GetKeyDown(KeyCode.R))
        {
            TornarAlCheckpointImmediat();
        }
    }

    public void SetNewCheckpoint(Vector3 posicio)
    {
        ultimCheckpoint = posicio;
    }

    // Aquesta funció es dispara quan toquem un obstacle
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") && !estaMort)
        {
            StartCoroutine(ProcesMort());
        }
    }

    // També per si l'obstacle no és Trigger (col·lisió física)
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
        
        // 1. Aturem el jugador i el fem invisible (opcional)
        rb.velocity = Vector2.zero;
        rb.simulated = false; // Això fa que no caigui ni xocqui mentre espera
        // GetComponent<SpriteRenderer>().enabled = false; // Si vols que desaparegui

        Debug.Log("Has mort! Reapareixent en " + tempsEsperaRespawn + " segons...");

        // 2. Esperem el temps que hagis posat a l'Inspector
        yield return new WaitForSeconds(tempsEsperaRespawn);

        // 3. Tornem al lloc
        transform.position = ultimCheckpoint;
        
        // 4. Resetegem l'estat
        rb.simulated = true;
        // GetComponent<SpriteRenderer>().enabled = true;
        estaMort = false;

        // Opcional: Si vols que al morir també recuperi la 'W' i els salts
        GetComponent<PlayerControllerScript>().ResetSaltsIAbilitat();
    }

    public void TornarAlCheckpointImmediat()
    {
        StopAllCoroutines(); // Per si morim i premem R a la vegada
        estaMort = false;
        rb.simulated = true;
        transform.position = ultimCheckpoint;
        rb.velocity = Vector2.zero;
        GetComponent<PlayerControllerScript>().ResetSaltsIAbilitat();
    }
}
