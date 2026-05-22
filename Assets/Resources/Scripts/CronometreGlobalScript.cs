using UnityEngine;

public class CronometreGlobalScript : MonoBehaviour
{
   
    public static float tempsJugat = 0f;
    public static bool estaActiu = false;

    [Header("Configuració Inici")]
    [Tooltip("Marca aquesta casella NOMÉS a la primera escena del joc per posar el temps a 0")]
    public bool iniciarComptadorAqui = false;

    void Start()
    {
        if (iniciarComptadorAqui)
        {
            tempsJugat = 0f;
            estaActiu = true;
        }
    }

    void Update()
    {
        if (estaActiu)
        {
            tempsJugat += Time.deltaTime;
        }
    }
}