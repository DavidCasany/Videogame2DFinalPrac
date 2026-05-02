using UnityEngine;
using UnityEngine.SceneManagement; 

public class IntroChangeSceneScript : MonoBehaviour
{

    public float tempsPerCanviar = 4f;

    void Start()
    {
        // Quan l'escena (i la cinemàtica) comenci, iniciem el compte enrere
        Invoke("CanviarEscena", tempsPerCanviar);
    }

    private void CanviarEscena()
    {
        // Carreguem la nova escena
        SceneManager.LoadScene(2);
    }
}