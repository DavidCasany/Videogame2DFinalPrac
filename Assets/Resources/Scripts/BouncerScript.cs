using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerScript : MonoBehaviour
{

    [Header("Configuració del Rebot")]
    public float forçaDeRebot = 20f;
    
    // Opcional: una animació per quan reboti
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Comprovem si el que ha tocat l'objecte és el jugador
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            PlayerControllerScript controller = other.GetComponent<PlayerControllerScript>();

            if (rb != null)
            {
                // 1. Resetegem la velocitat vertical per evitar acumulacions estranyes
                rb.velocity = new Vector2(rb.velocity.x, 0);

                // 2. Apliquem l'impuls cap amunt
                rb.AddForce(Vector2.up * forçaDeRebot, ForceMode2D.Impulse);

                // 3. IMPORTANT: Resetejem els salts i l'habilitat W del jugador
                if (controller != null)
                {
                    controller.ResetSaltsIAbilitat();
                }

                // 4. Si tens animació de "boing", la disparem
                if (anim != null) anim.SetTrigger("Rebot");
                
                Debug.Log("Rebot efectuat!");
            }
        }
    }
}
