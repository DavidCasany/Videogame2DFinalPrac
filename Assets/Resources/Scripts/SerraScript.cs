using UnityEngine;

public class SerraScript : MonoBehaviour
{
    [Header("Configuració de Moviment")]
    [Tooltip("Marca aquesta casella si vols que la serra es quedi quieta al seu lloc.")]
    public bool esEstatica = false;
    
    public Transform puntA;
    public Transform puntB;
    public float velocitat = 3f;


    private Vector3 proximObjectiu;
    private Animator ar;
    private bool estaEnMarxa = true; 

    void Start()
    {
        ar = GetComponent<Animator>();

       
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
  
        bool animacioActualEsIdle = ar != null && ar.GetCurrentAnimatorStateInfo(0).IsName("Serra_idle");

        if (!esEstatica && estaEnMarxa && animacioActualEsIdle && puntA != null && puntB != null)
        {
            MoureSerra();
        }
  
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


    public void Parar()
    {
        estaEnMarxa = false;
        
        if (ar != null)
        {
            ar.SetInteger("State", 1); 
        }
        
        Debug.Log("La serra s'ha aturat.");
    }

    public void Engegar()
    {
        estaEnMarxa = true;
        
        if (ar != null)
        {
            ar.SetInteger("State", 2); 
        }
        
        Debug.Log("La serra s'ha engegat.");
    }
}