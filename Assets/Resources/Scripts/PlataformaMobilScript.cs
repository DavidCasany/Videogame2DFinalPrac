using UnityEngine;

public class PlataformaMobilScript : MonoBehaviour
{
    public Transform puntA;
    public Transform puntB;
    public float velocitat = 3f;

    [Header("Configuració")]
    [Tooltip("Si està marcat, el jugador pot saltar des de sota per travessar-la.")]
    public bool esAtrabessable = true;

    private BoxCollider2D floor;
    private Vector3 proximObjectiu;

    void Start()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();

        // Si tenim més d'un collider (el trigger i el sòlid), busquem el sòlid
        if (colliders.Length > 1)
        {
            floor = colliders[0].isTrigger ? colliders[1] : colliders[0];
        }
        else
        {
            // Si només en té un (perquè no és travessable), agafem aquest
            floor = colliders[0];
        }

        // Desvinculem els punts per seguretat
        if (puntA.parent == transform) puntA.SetParent(null);
        if (puntB.parent == transform) puntB.SetParent(null);

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

    // --- LÒGICA PER MOURE EL JUGADOR AMB LA PLATAFORMA ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    // --- LÒGICA PER TRAVESSAR LA PLATAFORMA ---
    void OnTriggerStay2D(Collider2D collision)
    {
        // Si la plataforma no és travessable, ignorem completament aquesta funció
        if (!esAtrabessable) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();

            // Si el jugador va cap amunt (saltant des de sota), apaguem la col·lisió sòlida
            if (rbJugador.velocity.y > 0.0f)
            {
                floor.enabled = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Si no és travessable, ignorem la funció
        if (!esAtrabessable) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Quan el jugador surt del tot del trigger, tornem a encendre el terra sòlid
            if (floor != null)
            {
                floor.enabled = true;
            }
        }
    }
}