using UnityEngine;

public class WallBladeScript : MonoBehaviour
{
    private Animator ar;

    void Start()
    {
        ar = GetComponent<Animator>();

     
        if (ar != null)
        {
            ar.SetBool("Active", true);
        }
    }

    
    public void CanviarEstatTrampa(bool estat)
    {
        if (ar != null)
        {
            ar.SetBool("Active", estat);
        }

 
    }
}