using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalTutorialScript : MonoBehaviour
{
    [Header("Elements Visuals i UI")]
    public GameObject iconaInteraccio;
    public GameObject panelText;

    [Header("Monitor Interactiu")]
    public Animator monitorAnimator;

    [Header("Recompensa i Escena")]
    public GameObject consumiblePrefab;
    public float yOffsetBateria = 2f; 
    public int indexEscenaSeguent = 3; 


    private bool jugadorAProp = false;
    private bool llegintDialeg = false;
    private PlayerControllerScript playerScript;

    void OnEnable()
    {
        if (iconaInteraccio != null) iconaInteraccio.SetActive(true);
        if (panelText != null) panelText.SetActive(false);
        if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", true);

        llegintDialeg = false;
    }

    void Update()
    {
      
        if (jugadorAProp && !llegintDialeg && Input.GetKeyDown(KeyCode.E))
        {
            llegintDialeg = true;

            if (iconaInteraccio != null) iconaInteraccio.SetActive(false);
            if (panelText != null) panelText.SetActive(true);
            if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", false);

           
            if (playerScript != null)
            {
                playerScript.DesactivarControls();
            }
        }

       
        if (llegintDialeg && panelText != null && !panelText.activeSelf)
        {
            llegintDialeg = false; 
            if (consumiblePrefab != null && playerScript != null)
            {
                Vector3 spawnPos = new Vector3(
                    playerScript.transform.position.x,
                    playerScript.transform.position.y + yOffsetBateria,
                    0
                );
                Instantiate(consumiblePrefab, spawnPos, Quaternion.identity);
            }

            if (monitorAnimator != null) monitorAnimator.SetBool("ScreenRed", true);

           
            Invoke("FinalitzarEscenaTutorial", 2f);
        }
    }

    private void FinalitzarEscenaTutorial()
    {
        SceneManager.LoadScene(indexEscenaSeguent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = true;
          
            playerScript = collision.GetComponent<PlayerControllerScript>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = false;
        }
    }
}