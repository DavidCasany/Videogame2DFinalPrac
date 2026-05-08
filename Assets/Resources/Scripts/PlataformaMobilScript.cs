using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaMobilScript : MonoBehaviour
{

    public Transform puntA;      
    public Transform puntB;      
    public float velocitat = 3f; 
    BoxCollider2D floor;

    private Vector3 proximObjectiu;

    void Start()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        floor = colliders[0].isTrigger ? colliders[1] : colliders[0];
        
        proximObjectiu = puntB.position;
    }

    void Update()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, proximObjectiu, velocitat * Time.deltaTime);

        
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

    // AQUESTA PART �S CRUCIAL: Fa que el jugador es mogui AMB la plataforma
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.y>0.0f && collision.gameObject.CompareTag("Player"))
        {
            floor.enabled = false;
        }
        else
        {
            floor.enabled = true;
        }
    }

}
