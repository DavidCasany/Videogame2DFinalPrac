using System.Collections; // Necessari per a les corrutines
using UnityEngine;

public class KamikazeScript : MonoBehaviour
{
    [Header("Configuració de Moviment")]
    public float velocitatDeambular = 1.5f;
    public float velocitatPersecucio = 3.5f;

    [Header("Configuració de Deambular")]
    public float radiDeambular = 4f;
    public float tempsEsperaWander = 1.5f;
    public float tempsMaximBloqueig = 3f;

    [Header("Detecció i Atac")]
    public float radiVisio = 6f;
    public float radiAtac = 1.5f;
    public float tempsCompteEnrere = 1.5f;

    [Header("Explosions i Combat")]
    public GameObject prefabExplosioLetal;
    public GameObject prefabExplosioInofensiva;
    public float forcaRebotJugador = 15f;
    [Tooltip("El temps que es mostra l'animació de rebre dany abans d'explotar visualment")]
    public float tempsAnimacioDany = 0.3f; // NOU

    // Variables internes
    private Transform jugador;
    private Animator ar;
    private Rigidbody2D rb;
    private Vector3 puntObjectiu;
    private Vector3 posicioInicial;
    private float temporitzadorEspera = 0f;
    private float temporitzadorAntiBloqueig = 0f;
    private float temporitzadorAtac = 0f;
    private bool mirantDreta = true;
    private bool esMort = false; // NOU: Assegura que només mori un cop

    // NOU: Hem afegit l'estat Danyat
    private enum EstatIA { Deambulant, Perseguint, Atacant, Danyat }
    private EstatIA estatActual;

    void Start()
    {
        ar = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        posicioInicial = transform.position;

        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }

        TriarNouPuntDeambular();
    }

    void Update()
    {
        // 1. Si ja l'hem matat, que ignori qualsevol altra decisió d'IA
        if (esMort || estatActual == EstatIA.Danyat) return;

        if (jugador == null)
        {
            if (estatActual != EstatIA.Atacant) ComportamentDeambular();
            return;
        }

        if (estatActual == EstatIA.Atacant)
        {
            ComportamentAtac();
            return;
        }

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaAlJugador <= radiAtac)
        {
            estatActual = EstatIA.Atacant;
        }
        else if (distanciaAlJugador <= radiVisio)
        {
            estatActual = EstatIA.Perseguint;
        }
        else
        {
            estatActual = EstatIA.Deambulant;
        }

        if (estatActual == EstatIA.Deambulant)
        {
            ComportamentDeambular();
        }
        else if (estatActual == EstatIA.Perseguint)
        {
            ComportamentPersecucio();
        }
    }

    private void ComportamentDeambular()
    {
        if (temporitzadorEspera > 0)
        {
            temporitzadorEspera -= Time.deltaTime;
            rb.velocity = Vector2.zero; // MODIFICAT: S'atura completament
            CanviarAnimacio(0);
            return;
        }

        MoureCapA(puntObjectiu, velocitatDeambular);
        temporitzadorAntiBloqueig += Time.deltaTime;

        bool haArribatAlPunt = Vector2.Distance(transform.position, puntObjectiu) < 0.1f;
        bool estaEncallat = temporitzadorAntiBloqueig >= tempsMaximBloqueig;

        if (haArribatAlPunt || estaEncallat)
        {
            temporitzadorEspera = tempsEsperaWander;
            TriarNouPuntDeambular();
        }
    }

    private void ComportamentPersecucio()
    {
        temporitzadorEspera = 0f;
        MoureCapA(jugador.position, velocitatPersecucio);
    }

    private void ComportamentAtac()
    {
        rb.velocity = Vector2.zero; // MODIFICAT: S'atura totalment en l'aire per fer el compte enrere
        CanviarAnimacio(2);
        temporitzadorAtac += Time.deltaTime;

        if (temporitzadorAtac >= tempsCompteEnrere)
        {
            InstanciarIAutodestruir(prefabExplosioLetal);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si ja l'hem matat, ignorem nous xocs
        if (esMort || estatActual == EstatIA.Danyat) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerControllerScript playerScript = collision.gameObject.GetComponent<PlayerControllerScript>();

            if (playerScript != null)
            {
                if (playerScript.estaFentDobleSalt)
                {
                    // MATEM L'ENEMIC
                    esMort = true;
                    estatActual = EstatIA.Danyat;

                    // L'impuls al jugador (per rebotar)
                    playerScript.AplicarRebotAlMatarEnemic(forcaRebotJugador);

                    // Iniciem la corrutina de rebre dany
                    StartCoroutine(ProcesMortEnemic());
                }
                else
                {
                    // Aquest text sortirà a la consola si hi xoques i no funciona
                    Debug.Log("Has xocat, però l'script del jugador diu que NO estaves fent doble salt!");
                }
            }
        }
    }

    // NOU: Procés de mort amb animació prèvia
    private IEnumerator ProcesMortEnemic()
    {
        // 1. Aturem l'enemic perquè no empenyi més
        rb.velocity = Vector2.zero;

        // 2. Desactivem el collider perquè el jugador no rebi xocs rars mentre l'enemic fa l'animació
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 3. Posem l'animació de rebre dany (State 3)
        CanviarAnimacio(3);

        // 4. Esperem un instant perquè el jugador pugui gaudir de l'animació
        yield return new WaitForSeconds(tempsAnimacioDany);

        // 5. Creem l'explosió inofensiva i destruïm el robot
        InstanciarIAutodestruir(prefabExplosioInofensiva);
    }

    private void InstanciarIAutodestruir(GameObject explosio)
    {
        if (explosio != null)
        {
            Instantiate(explosio, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    // MODIFICAT: Ara utilitzem la velocitat de les físiques en lloc de teletransportar la posició
    private void MoureCapA(Vector3 desti, float velocitat)
    {
        Vector2 direccio = (desti - transform.position).normalized;
        rb.velocity = direccio * velocitat;

        CanviarAnimacio(1);
        GirarSprite(desti.x);
    }

    private void TriarNouPuntDeambular()
    {
        Vector2 puntAleatori = Random.insideUnitCircle * radiDeambular;
        puntObjectiu = posicioInicial + new Vector3(puntAleatori.x, puntAleatori.y, 0);
        temporitzadorAntiBloqueig = 0f;
    }

    private void CanviarAnimacio(int estat)
    {
        if (ar != null)
        {
            ar.SetInteger("State", estat);
        }
    }

    private void GirarSprite(float destiX)
    {
        if (destiX > transform.position.x && !mirantDreta)
        {
            mirantDreta = true;
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (destiX < transform.position.x && mirantDreta)
        {
            mirantDreta = false;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiVisio);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiAtac);
    }
}