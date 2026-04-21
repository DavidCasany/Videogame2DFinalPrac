using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaMobilScript : MonoBehaviour
{

    public Transform puntA;      // On comença
    public Transform puntB;      // On va
    public float velocitat = 3f; // Què tan ràpid es mou

    private Vector3 proximObjectiu;

    void Start()
    {
        // Comencem anant cap al punt B
        proximObjectiu = puntB.position;
    }

    void Update()
    {
        // Movem la plataforma cap a l'objectiu
        transform.position = Vector3.MoveTowards(transform.position, proximObjectiu, velocitat * Time.deltaTime);

        // Si hem arribat molt a prop de l'objectiu, canviem de sentit
        if (Vector3.Distance(transform.position, proximObjectiu) < 0.1f)
        {
            if (proximObjectiu == puntB.position)
            {
                proximObjectiu = puntA.position;
            }
            else
            {
                proximObjectiu = puntB.position;
            }
        }
    }

    // AQUESTA PART ÉS CRUCIAL: Fa que el jugador es mogui AMB la plataforma
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Fem que el jugador sigui "fill" de la plataforma
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Quan salta o marxa, el treiem de "fill" de la plataforma
            collision.transform.SetParent(null);
        }
    }

}
