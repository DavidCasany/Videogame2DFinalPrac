using UnityEngine;
using UnityEngine.SceneManagement; 

public class IntroChangeSceneScript : MonoBehaviour
{

    public float tempsPerCanviar = 4f;

    void Start()
    {
      
        Invoke("CanviarEscena", tempsPerCanviar);
    }

    private void CanviarEscena()
    {
     
        SceneManager.LoadScene(2);
    }
}