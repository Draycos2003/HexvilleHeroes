using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Sound Clips")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip hoverSound;

    [Header("Sound Settings")]
    [Range(0f, 1f)][SerializeField] private float clickVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float hoverVolume = 1f;

    private bool hasPlayedHover = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!hasPlayedHover && hoverSound != null && soundFXmanager.instance != null)
        {
            soundFXmanager.instance.PlaySoundFXClip(hoverSound, transform, hoverVolume);
            hasPlayedHover = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hasPlayedHover = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && soundFXmanager.instance != null)
        {
            soundFXmanager.instance.PlaySoundFXClip(clickSound, transform, clickVolume);
        }
    }

    private void OnDisable()
    {
        hasPlayedHover = false;
    }
}
