using UnityEngine;
using TMPro;

public class MostraPuntuacioScript : MonoBehaviour
{
    [Header("Element de Text UI (TextMeshPro)")]
    public TextMeshProUGUI textPuntuacio;

    void OnEnable()
    {
        CronometreGlobalScript.estaActiu = false;

        if (textPuntuacio != null)
        {
            textPuntuacio.text = "TEMPS: " + FormatejarTemps(CronometreGlobalScript.tempsJugat);
        }
    }

    private string FormatejarTemps(float temps)
    {
        int minuts = Mathf.FloorToInt(temps / 60f);
        int segons = Mathf.FloorToInt(temps - minuts * 60f);
        int milisegons = Mathf.FloorToInt((temps - minuts * 60f - segons) * 100f);

        return string.Format("{0:00}:{1:00}:{2:00}", minuts, segons, milisegons);
    }
}