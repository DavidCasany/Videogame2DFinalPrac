using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class DialegTextScript : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI textComponent;
    public TextAsset arxiuText;

    [Header("Configuració Visual")]
    public float tempsPerLletra = 0.05f;
    public Color colorDestacat = Color.yellow;

    [Header("Avanç del Diàleg")]
    [Tooltip("Si és cert, passa a la següent línia sol. Si és fals, espera la tecla.")]
    public bool avancarAutomaticament = true;
    public float tempsEsperaAutomatic = 2f;

    void OnEnable()
    {
        if (arxiuText != null && textComponent != null)
        {
            StartCoroutine(LlegirArxiu());
        }
        else
        {
            Debug.LogWarning("Falta assignar el TextMeshPro o l'arxiu .txt a l'Inspector!");
        }
    }

    IEnumerator LlegirArxiu()
    {
        string codiColorHex = "#" + ColorUtility.ToHtmlStringRGB(colorDestacat);
        string[] linies = arxiuText.text.Split('\n');

        foreach (string linia in linies)
        {
            string liniaNeta = linia.Trim();
            if (string.IsNullOrEmpty(liniaNeta)) continue;

            string liniaFormatejada = Regex.Replace(liniaNeta, @"#(.*?)#", $"<color={codiColorHex}>$1</color>");

            textComponent.text = liniaFormatejada;
            textComponent.ForceMeshUpdate();
            int totalCaracters = textComponent.textInfo.characterCount;
            textComponent.maxVisibleCharacters = 0;

            for (int i = 0; i <= totalCaracters; i++)
            {
                textComponent.maxVisibleCharacters = i;
                yield return new WaitForSeconds(tempsPerLletra);
            }

            if (avancarAutomaticament)
            {
                yield return new WaitForSeconds(tempsEsperaAutomatic);
            }
        }

        textComponent.text = "";

        // NOU: Desactiva l'objecte automàticament quan s'acaben les línies
        gameObject.SetActive(false);
    }
}