using UnityEngine;
using UnityEngine.Events;


public class TerminalBossScript : MonoBehaviour
{
    [Header("Elements Visuals")]
    public GameObject iconaInteraccio;

    [Header("Acció Boss")]
    public UnityEvent enActivar;

    [Header("Recompensa (Bateria)")]
    public GameObject consumiblePrefab;
    public float yOffsetBateria = 1.5f;

    private bool jugadorAProp = false;
    private bool estaActivat = false;

    void Start()
    {
        if (iconaInteraccio != null) iconaInteraccio.SetActive(false);
    }

    void Update()
    {
        if (jugadorAProp && !estaActivat && Input.GetKeyDown(KeyCode.E))
        {
            ActivarTerminal();
        }
    }

    private void ActivarTerminal()
    {
        estaActivat = true;

        if (iconaInteraccio != null) iconaInteraccio.SetActive(false);

        SpawnejarBateria();

        if (enActivar != null)
        {
            enActivar.Invoke();
        }
    }

    private void SpawnejarBateria()
    {
        if (consumiblePrefab != null)
        {
            Vector3 spawnPos = new Vector3(
                transform.position.x,
                transform.position.y + yOffsetBateria,
                transform.position.z
            );

            Instantiate(consumiblePrefab, spawnPos, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !estaActivat)
        {
            jugadorAProp = true;
            if (iconaInteraccio != null) iconaInteraccio.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorAProp = false;
            if (iconaInteraccio != null) iconaInteraccio.SetActive(false);
        }
    }
}