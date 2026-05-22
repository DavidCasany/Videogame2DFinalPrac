using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Aquest script s'assigna als botons de la interfície d'usuari.
/// Detecta les interaccions del ratolí i del teclat per invocar els efectes
/// de so pertinents a través del gestor actiu en l'escena (Menú Principal o Pausa).
/// </summary>
public class BotonsMenuScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    private MenuPrincipalScript menuManager;
    private MenuPausaScript pausaManager;
    private bool ratoliAProp = false;

    void Start()
    {
        menuManager = FindFirstObjectByType<MenuPrincipalScript>();
        pausaManager = FindFirstObjectByType<MenuPausaScript>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ratoliAProp = true;

        if (menuManager != null) menuManager.ReproduirSoHover();
        if (pausaManager != null) pausaManager.ReproduirSoHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ratoliAProp = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!ratoliAProp)
        {
            if (menuManager != null) menuManager.ReproduirSoHover();
            if (pausaManager != null) pausaManager.ReproduirSoHover();
        }
    }
}