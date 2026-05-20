using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnergiaControllerScript : MonoBehaviour
{
    [Header("Configuració de la UI")]
    public Image imatgeBarraEnergia;
    public Sprite[] spritesBarra;

    [Header("Configuració de Temps")]
    public float tempsMaximEnergia = 60f;
    public float tempsAnimacioRecarrega = 1f;

    private float energiaActual;
    private bool isRecarregant = false;
    private bool pausaExterna = false;

    void Start()
    {
        energiaActual = tempsMaximEnergia;
        ActualitzarSpriteBarra();
    }

    void Update()
    {
        // Només gastem l'energia si NO estem enmig de l'animació de recàrrega
        if (energiaActual > 0 && !isRecarregant && !pausaExterna)
        {
            energiaActual -= Time.deltaTime;
            ActualitzarSpriteBarra();

            if (energiaActual <= 0)
            {
                energiaActual = 0;
                QuedarSenseEnergia();
            }
        }
    }

    void ActualitzarSpriteBarra()
    {
        if (spritesBarra.Length == 0 || imatgeBarraEnergia == null) return;

        int passosDeBateria = spritesBarra.Length - 1;
        float proporcioEnergia = energiaActual / tempsMaximEnergia;
        int indexSprite = Mathf.CeilToInt(proporcioEnergia * passosDeBateria);
        indexSprite = Mathf.Clamp(indexSprite, 0, passosDeBateria);

        imatgeBarraEnergia.sprite = spritesBarra[indexSprite];
    }

    public void RecarregarEnergia()
    {
        if (!isRecarregant)
        {
            StartCoroutine(AnimacioRecarregaRutina());
        }
    }

    private IEnumerator AnimacioRecarregaRutina()
    {
        isRecarregant = true;
        float energiaInicial = energiaActual;
        float tempsPassat = 0f;

        while (tempsPassat < tempsAnimacioRecarrega)
        {
            tempsPassat += Time.deltaTime;
            float proporcioTemps = tempsPassat / tempsAnimacioRecarrega;
            energiaActual = Mathf.Lerp(energiaInicial, tempsMaximEnergia, proporcioTemps);
            ActualitzarSpriteBarra();
            yield return null;
        }

        energiaActual = tempsMaximEnergia;
        ActualitzarSpriteBarra();
        isRecarregant = false;
    }

    void QuedarSenseEnergia()
    {
        Debug.Log("Sense bateria! Has mort.");

        // Intentem trobar el jugador (tant si l'script està al player com si està a la UI)
        PlayerControllerScript player = GetComponent<PlayerControllerScript>();
        if (player == null)
        {
            player = FindObjectOfType<PlayerControllerScript>();
        }

        if (player != null)
        {
            Debug.Log("Enviant ordre de morir al jugador...");
            player.Morir();
        }
        else
        {
            Debug.LogError("ERROR: No s'ha trobat el PlayerControllerScript per executar la mort!");
        }
    }
   
    public void SetPausaEnergia(bool estat)
    {
        pausaExterna = estat;
    }
}