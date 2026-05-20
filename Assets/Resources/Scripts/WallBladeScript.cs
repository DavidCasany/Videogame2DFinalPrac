using UnityEngine;

public class WallBladeScript : MonoBehaviour
{
    private Animator ar;

    void Start()
    {
        ar = GetComponent<Animator>();

        // Per defecte, la trampa comença activa (movent-se)
        if (ar != null)
        {
            ar.SetBool("Active", true);
        }
    }

    // --- FUNCIÓ PÚBLICA PER ACTIVAR O DESACTIVAR LA TRAMPA ---
    public void CanviarEstatTrampa(bool estat)
    {
        if (ar != null)
        {
            ar.SetBool("Active", estat);
        }

 
    }
}