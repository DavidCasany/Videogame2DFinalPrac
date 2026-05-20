using UnityEngine;

public class SerraScript : MonoBehaviour
{
    [Header("Configuració de Moviment")]
    [Tooltip("Marca aquesta casella si vols que la serra es quedi quieta al seu lloc.")]
    public bool esEstatica = false;
    
    public Transform puntA;
    public Transform puntB;
    public float velocitat = 3f;

    // Variables de control intern
    private Vector3 proximObjectiu;
    private Animator ar;
    private bool estaEnMarxa = true; 

    void Start()
    {
        ar = GetComponent<Animator>();

        // Sempre comencem enviant el paràmetre Idle (0)
        if (ar != null)
        {
            ar.SetInteger("State", 0);
        }

        if (!esEstatica)
        {
            if (puntA != null && puntA.parent == transform) puntA.SetParent(null);
            if (puntB != null && puntB.parent == transform) puntB.SetParent(null);

            if (puntB != null) proximObjectiu = puntB.position;
        }
    }

    void Update()
    {
        // 1. COMPROVACIÓ REALS DE L'ANIMACIÓ:
        // Mirem si l'animació que està sonant ARA MATEIX a Unity es diu "Idle"
        // (Assegura't que el bloc de l'animació a l'Animator es digui exactament "Idle")
        bool animacioActualEsIdle = ar != null && ar.GetCurrentAnimatorStateInfo(0).IsName("Serra_idle");

        // 2. CONDICIÓ DE MOVIMENT:
        // Només es mou si NO és estàtica, la serra està en marxa i l'animació activa és Idle
        if (!esEstatica && estaEnMarxa && animacioActualEsIdle && puntA != null && puntB != null)
        {
            MoureSerra();
        }
        // Si no es compleix (és estàtica, o l'animació és Engegar(2) o Aturar(1)), 
        // el codi no farà res i la serra es quedarà completament quieta al seu lloc.
    }

    private void MoureSerra()
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

    // --- FUNCIONS PÚBLIQUES D'ANIMACIÓ ---

    public void Parar()
    {
        estaEnMarxa = false;
        
        if (ar != null)
        {
            ar.SetInteger("State", 1); // Estat 1: Parar (Aturarà el moviment immediatament)
        }
        
        Debug.Log("La serra s'ha aturat.");
    }

    public void Engegar()
    {
        estaEnMarxa = true;
        
        if (ar != null)
        {
            ar.SetInteger("State", 2); // Estat 2: Engegar
        }
        
        Debug.Log("La serra s'ha engegat.");
    }
}