using UnityEngine;
using UnityEngine.Events;

public class TerminalScript : MonoBehaviour
{
    [Header("Elements Visuals")]
    public GameObject iconaInteraccio;
  
    [Header("Accions Universals")]
    [Tooltip("Què passa quan l'interruptor s'ACTIVA?")]
    public UnityEvent enActivar;

    [Tooltip("Què passa quan l'interruptor es DESACTIVA?")]
    public UnityEvent enDesactivar;

    // Variables de control intern (Amagades de l'Inspector)
    private bool jugadorAProp = false;
    private bool estaActivat = false; // Comença desactivat per defecte

    void Start()
    {
        if (iconaInteraccio != null) iconaInteraccio.SetActive(false);

      
    }

    void Update()
    {
        if (jugadorAProp && Input.GetKeyDown(KeyCode.E))
        {
            CanviarEstat();
        }
    }

    private void CanviarEstat()
    {
        estaActivat = !estaActivat;

        if (estaActivat)
        {
            enActivar.Invoke();
        }
        else
        {
            enDesactivar.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = true;
            if (iconaInteraccio != null) iconaInteraccio.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = false;
            if (iconaInteraccio != null) iconaInteraccio.SetActive(false);
        }
    }
}