using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuPausaScript : MonoBehaviour
{
    [Header("Interfície")]
    public GameObject panellPausa;

    [Header("Sons dels Botons")]
    public AudioSource sfxAudioSource;
    public AudioClip soHover;
    public AudioClip soClick;

    [Range(0f, 1f)]
    public float volumSFX = 0.7f;

    private bool jocPausat = false;
    private bool bloquejarClics = false;

    void Start()
    {
        panellPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !bloquejarClics)
        {
            if (jocPausat) ReprendreJoc();
            else PausarJoc();
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

    public void PausarJoc()
    {
        panellPausa.SetActive(true);
        Time.timeScale = 0f;
        jocPausat = true;
    }

    public void ReprendreJoc()
    {
        if (bloquejarClics) return;

        ReproduirSoClick();
        panellPausa.SetActive(false);
        Time.timeScale = 1f;
        jocPausat = false;
    }

    public void SortirAlMenuPrincipal()
    {
        if (bloquejarClics) return;

        bloquejarClics = true;
        ReproduirSoClick();
        StartCoroutine(SortirAmbRetard());
    }

    private IEnumerator SortirAmbRetard()
    {
        Time.timeScale = 1f;

        float retard = (soClick != null) ? soClick.length : 0.4f;
        yield return new WaitForSeconds(retard);

        SceneManager.LoadScene(0);
    }
}