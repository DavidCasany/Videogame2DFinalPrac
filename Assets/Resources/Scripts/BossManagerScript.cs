using System.Collections;
using UnityEngine;

public class BossManagerScript : MonoBehaviour
{
    [Header("Interfície de Diàleg")]
    public GameObject marcaDialog;
    public GameObject panelText;

    [Header("Elements del Combat")]
    public GameObject contenidorObstacles;
    public GameObject enemicPrefab;
    public Transform[] puntsSpawnEnemic;
    public float tempsEntreSpawns = 10f;
    public int totalEnemicsPerGuanyar = 10;

    [Header("Àudio, Animació i UI Final")]
    public AudioSource musicaCombat;
    public Animator bossAnimator;
    public float duradaAnimacioMort = 2f;
    public GameObject victoriaUI;

    private bool jugadorAProp = false;
    private bool llegintDialeg = false;
    private bool combatIniciat = false;
    private int enemicsDestruits = 0;
    private Coroutine rutinaSpawn;
    private bool bossMort = false;

    void Start()
    {
        if (marcaDialog != null) marcaDialog.SetActive(false);
        if (panelText != null) panelText.SetActive(false);
        if (contenidorObstacles != null) contenidorObstacles.SetActive(false);
        if (victoriaUI != null) victoriaUI.SetActive(false);
        if (bossAnimator != null) bossAnimator.SetInteger("State", 0);
    }

    void Update()
    {
        if (marcaDialog != null)
        {
            marcaDialog.SetActive(jugadorAProp && !combatIniciat && !llegintDialeg);
        }

        if (jugadorAProp && !combatIniciat && !llegintDialeg && Input.GetKeyDown(KeyCode.E))
        {
            ComencarDialeg();
        }

        if (llegintDialeg && panelText != null && !panelText.activeSelf)
        {
            FinalitzarDialeg();
        }
    }

    void ComencarDialeg()
    {
        llegintDialeg = true;
        if (marcaDialog != null) marcaDialog.SetActive(false);
        if (panelText != null) panelText.SetActive(true);
    }

    void FinalitzarDialeg()
    {
        llegintDialeg = false;
        combatIniciat = true;

        if (contenidorObstacles != null) contenidorObstacles.SetActive(true);
        if (musicaCombat != null) musicaCombat.Play();
        if (bossAnimator != null) bossAnimator.SetInteger("State", 1);

        rutinaSpawn = StartCoroutine(SpawnEnemicsContinu());
    }

    private IEnumerator SpawnEnemicsContinu()
    {
        while (enemicsDestruits < totalEnemicsPerGuanyar)
        {
            SpawnEnemicAleatori();
            yield return new WaitForSeconds(tempsEntreSpawns);
        }
    }

    private void SpawnEnemicAleatori()
    {
        if (enemicPrefab != null && puntsSpawnEnemic != null && puntsSpawnEnemic.Length > 0)
        {
            int index = Random.Range(0, puntsSpawnEnemic.Length);
            Transform puntSortejat = puntsSpawnEnemic[index];
            if (puntSortejat != null)
            {
                Instantiate(enemicPrefab, puntSortejat.position, Quaternion.identity);
            }
        }
    }

    public void NotificarEnemicDestruit()
    {
        if (bossMort) return;

        enemicsDestruits++;

        if (enemicsDestruits >= totalEnemicsPerGuanyar)
        {
            ExecutarMortBoss();
        }
    }

    private void ExecutarMortBoss()
    {
        bossMort = true;
        if (rutinaSpawn != null) StopCoroutine(rutinaSpawn);

        if (bossAnimator != null) bossAnimator.SetInteger("State", 2);

        StartCoroutine(DestruirDespresDeAnimacio());
    }

    private IEnumerator DestruirDespresDeAnimacio()
    {
        yield return new WaitForSeconds(duradaAnimacioMort);

        KamikazeScript[] enemicsRestants = FindObjectsOfType<KamikazeScript>();
        foreach (KamikazeScript enemic in enemicsRestants)
        {
            Destroy(enemic.gameObject);
        }

        if (contenidorObstacles != null) contenidorObstacles.SetActive(false);

        if (victoriaUI != null) victoriaUI.SetActive(true);

        Time.timeScale = 0f;

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = true;
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