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

    
        if (colliders.Length > 1)
        {
            floor = colliders[0].isTrigger ? colliders[1] : colliders[0];
        }
        else
        {
          
            floor = colliders[0];
        }

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

  
    void OnTriggerStay2D(Collider2D collision)
    {
     
        if (!esAtrabessable) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbJugador = collision.gameObject.GetComponent<Rigidbody2D>();

         
            if (rbJugador.velocity.y > 0.0f)
            {
                floor.enabled = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {

        if (!esAtrabessable) return;

        if (collision.gameObject.CompareTag("Player"))
        {
      
            if (floor != null)
            {
                floor.enabled = true;
            }
        }
    }
}