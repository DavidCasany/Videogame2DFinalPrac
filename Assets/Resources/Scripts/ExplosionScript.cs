using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    [Tooltip("El temps exacte que dura l'animació de l'explosió en segons")]
    public float tempsDeVida = 0.5f;

    void Start()
    {
       
        Destroy(gameObject, tempsDeVida);
    }
}