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

        // Només començar, llegim com has deixat la casella a l'Inspector i ho apliquem
        AplicarEstat();
    }



    // (Opcional) Funció extra per si mai vols un botó que simplement inverteixi l'estat actual
    public void AlternarEstat()
    {
        estatActiu = !estatActiu; // Si era true passa a false, i viceversa
        AplicarEstat();
    }

    // --- FUNCIÓ INTERNA CENTRALITZADA ---
    private void AplicarEstat()
    {
        // Aquesta funció s'encarrega de sincronitzar l'animació i la hitbox amb la teva checkbox
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