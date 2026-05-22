using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalScript : MonoBehaviour
{
    [Header("Sons dels Botons")]
    public AudioSource sfxAudioSource;
    public AudioClip soHover;
    public AudioClip soClick;

    [Range(0f, 1f)] 
    [Tooltip("Volum per als efectes de so dels botons (Hover i Clic).")]
    public float volumSFX = 0.7f;

    [Header("Música de Fons")]
    public AudioSource musicaAudioSource;

    [Range(0f, 1f)]
    [Tooltip("El volum màxim que agafarà la música de fons després del crescendo.")]
    public float volumMaximMusica = 0.5f;

    public float tempsFadeIn = 1.5f;

    private bool bloquejarClics = false;

    void Start()
    {
        if (musicaAudioSource != null)
        {
            musicaAudioSource.volume = 0f;
            musicaAudioSource.Play();
            StartCoroutine(FadeMusica(0f, volumMaximMusica, tempsFadeIn));
        }
    }



    public void ReproduirSoHover()
    {
        if (bloquejarClics) return;

        if (sfxAudioSource != null && soHover != null)
        {
         
            sfxAudioSource.PlayOneShot(soHover, volumSFX);
        }
    }

    private void ReproduirSoClick()
    {
        if (sfxAudioSource != null && soClick != null)
        {
            
            sfxAudioSource.PlayOneShot(soClick, volumSFX);
        }
    }

 

    public void StartNewGame()
    {
        if (bloquejarClics) return;
        bloquejarClics = true;

        ReproduirSoClick();
        StartCoroutine(CanviarEscenaAmbRetard(1));
    }

    public void QuitGame()
    {
        if (bloquejarClics) return;
        bloquejarClics = true;

        ReproduirSoClick();
        StartCoroutine(SortirAmbRetard());
    }

    private IEnumerator FadeMusica(float volumInici, float volumFi, float durada)
    {
        float temps = 0f;
        while (temps < durada)
        {
            temps += Time.deltaTime;
            musicaAudioSource.volume = Mathf.Lerp(volumInici, volumFi, temps / durada);
            yield return null;
        }
        musicaAudioSource.volume = volumFi;
    }

    private IEnumerator CanviarEscenaAmbRetard(int index)
    {
        float retard = (soClick != null) ? soClick.length : 0.4f;

        if (musicaAudioSource != null)
        {
            StartCoroutine(FadeMusica(musicaAudioSource.volume, 0f, retard));
        }

        yield return new WaitForSeconds(retard);
        SceneManager.LoadScene(index);
    }

    private IEnumerator SortirAmbRetard()
    {
        float retard = (soClick != null) ? soClick.length : 0.4f;

        if (musicaAudioSource != null)
        {
            StartCoroutine(FadeMusica(musicaAudioSource.volume, 0f, retard));
        }

        yield return new WaitForSeconds(retard);
        Application.Quit();
    }
}