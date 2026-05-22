using UnityEngine;

public class LaserScript : MonoBehaviour
{
    [Header("Estat del Làser")]
    [Tooltip("Activa o desactiva el làser. El Terminal modificarà aquesta casella en temps real.")]
    public bool estatActiu = true;

    private Animator ar;
    private Collider2D hitbox;

    void Start()
    {
        ar = GetComponent<Animator>();
        hitbox = GetComponent<Collider2D>();

        AplicarEstat();
    }




    public void AlternarEstat()
    {
        estatActiu = !estatActiu; 
        AplicarEstat();
    }


    private void AplicarEstat()
    {
    
        if (ar != null)
        {
            ar.SetBool("Active", estatActiu);
        }

        if (hitbox != null)
        {
            hitbox.enabled = estatActiu;
        }
    }
}