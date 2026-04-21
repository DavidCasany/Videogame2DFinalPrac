using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class ActivableScript : MonoBehaviour
{

    // Quan el jugador entra en el cercle/quadrat de l'objecte
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el que ens toca és el Jugador
        if (collision.CompareTag("Player"))
        {
            // Desactiva l'objecte (desapareix de la pantalla)
            gameObject.SetActive(false);
        }
    }


}
