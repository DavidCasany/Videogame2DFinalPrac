using UnityEngine;
using UnityEngine.SceneManagement; // Aquesta línia és OBLIGATÒRIA per canviar d'escena

public class MenuPrincipalScript : MonoBehaviour
{
    // Funció per al botó "New Game"
    public void StartNewGame()
    {
        // Carrega l'escena del joc (la número 2 als Build Settings)
        SceneManager.LoadScene(2);
    }

    // Funció per al botó "Options"
    public void OpenOptions()
    {
        // Carrega l'escena de les opcions (la número 1 als Build Settings)
        SceneManager.LoadScene(1);
    }

    // Funció per al botó "Quit"
    public void QuitGame()
    {
        // Tanca l'aplicació. 
        // NOTA: Això no funciona mentre jugues a l'Editor d'Unity, només a la build final (el joc exportat).
        Application.Quit();
        
        // Posem un Debug.Log per saber que funciona quan estem a l'Editor
        Debug.Log("Sortint del joc...");
    }
}