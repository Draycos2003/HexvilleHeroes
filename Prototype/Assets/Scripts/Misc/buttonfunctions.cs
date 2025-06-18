using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonFunctions : MonoBehaviour
{
    [Header("Tweens")]
    [SerializeField] private RectHeightAndAlphaTweener rectTweener;
    [SerializeField] private RectTransform backgroundImageRect;

    private gamemanager gm => gamemanager.instance;
    private playerController pc => gm.PlayerScript;

    private const float COLLAPSED_SHADE_HEIGHT = 435f;
    private const float EXPANDED_SHADE_HEIGHT = 1085f;
    private const float SHADE_TWEEN_DURATION = 0.5f;

    public void Resume()
    {
        gm.stateUnpause();
    }

    public void ToggleOptions()
    {
        bool isMainMenu = SceneManager.GetActiveScene().buildIndex == 0;
        bool wasOpen = gm.MenuOptions.activeSelf;

        if (isMainMenu)
        {
            if (!wasOpen)
            {
                // show shade
                gm.BackgroundShade.SetActive(true);
                if (rectTweener == null)
                    rectTweener = FindFirstObjectByType<RectHeightAndAlphaTweener>();

                rectTweener?.StartExpand();
                gm.MenuOptions.SetActive(true);
            }
            else
            {
                rectTweener?.CollapseInstant();
                StartCoroutine(CollapseShadeThenDisable());
                gm.MenuOptions.SetActive(false);
            }
            return;
        }

        // non-main scenes
        if (!gm.IsPaused)
            gm.statePause();

        // show shade
        gm.BackgroundShade.SetActive(true);

        var img = gm.BackgroundShade.GetComponent<Image>();
        if (img != null)
        {
            var c = img.color;
            c.a = 0.8f;
            img.color = c;
        }

        StartCoroutine(TweenShade(COLLAPSED_SHADE_HEIGHT, EXPANDED_SHADE_HEIGHT));
        if (rectTweener == null)
            rectTweener = FindFirstObjectByType<RectHeightAndAlphaTweener>();
        rectTweener?.StartExpand();

        gm.setActiveMenu(gm.MenuOptions);
    }


    private IEnumerator TweenShade(float fromH, float toH)
    {
        if (backgroundImageRect == null)
            yield break;

        float elapsed = 0f;
        Vector2 size = backgroundImageRect.sizeDelta;

        while (elapsed < SHADE_TWEEN_DURATION)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / SHADE_TWEEN_DURATION);
            backgroundImageRect.sizeDelta = new Vector2(size.x, Mathf.Lerp(fromH, toH, t));
            yield return null;
        }
        backgroundImageRect.sizeDelta = new Vector2(size.x, toH);
    }

    private IEnumerator CollapseShadeThenDisable()
    {
        yield return StartCoroutine(TweenShade(EXPANDED_SHADE_HEIGHT, COLLAPSED_SHADE_HEIGHT));
        gm.BackgroundShade.SetActive(false);
    }

    public void Restart()
    {
        var name = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(name);
        gm.stateUnpause();

        if (pc != null)
        {
            pc.HP = pc.MAXHPOrig;
            pc.Shield = pc.MAXShieldOrig;
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
        gm.stateUnpause();
    }

    public void ReturnToOriginalScene()
    {
        if (pc != null)
        {
            int orig = pc.GetOriginalSceneIndex();
            SceneManager.LoadScene(orig);
            gm.stateUnpause();
        }
    }

    public void LoadGame()
    {
        gm.Load();
    }

    public void BackFromOptions()
    {
        gm.setActiveMenu(gm.PauseMenu);
    }

    public void OnEquip() { }
    public void OnUse() { }
    public void OnDrop() { }
}
