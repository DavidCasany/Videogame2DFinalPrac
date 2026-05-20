using UnityEngine;

public class BateriaScript : MonoBehaviour
{
    // Aquesta funció s'activa quan algú entra al Trigger de la bateria
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Comprovem si qui ha tocat la bateria és el jugador
        if (collision.CompareTag("Player"))
        {
 
            EnergiaControllerScript energiaScript = FindObjectOfType<EnergiaControllerScript>();

            // 3. Si hem trobat l'script, cridem la funció de recarregar
            if (energiaScript != null)
            {
                energiaScript.RecarregarEnergia();
            }
            else
            {
                Debug.LogWarning("La bateria ha tocat el jugador, però no s'ha trobat l'EnergiaControllerScript!");
            }

            // 4. Destruïm l'objecte de la bateria perquè ja s'ha consumit
            Destroy(gameObject);
        }
    }
}