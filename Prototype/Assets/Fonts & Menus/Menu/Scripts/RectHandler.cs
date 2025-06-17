using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RectHeightAndAlphaTweener : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject Background;
    [SerializeField] private RectTransform targetRect;
    [SerializeField] private Image targetImage;

    [Header("Menu Roots")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsMenuUI;

    [Header("Buttons")]
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button optionsBackButton;

    [Header("Option ScrollViews")]
    [SerializeField] private List<GameObject> optionScrollViews = new List<GameObject>();

    [Header("Tween Settings")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float collapsedHeight = 435f;
    [SerializeField] private float expandedHeight = 1080f;
    [SerializeField][Range(0, 1f)] private float collapsedAlpha = 155f / 255f;
    [SerializeField][Range(0, 1f)] private float expandedAlpha = 200f / 255f;

    private Coroutine tweenCoroutine;
    [HideInInspector] public bool isFirstOpen = true;

    private void Awake()
    {
        optionsMenuUI.SetActive(false);
        foreach (var sv in optionScrollViews)
            sv.SetActive(false);

        optionsButton.onClick.AddListener(StartExpand);
        optionsBackButton.onClick.AddListener(StartCollapse);
    }

    public void CollapseInstant()
    {
        if (tweenCoroutine != null) StopCoroutine(tweenCoroutine);
        targetRect.sizeDelta = new Vector2(targetRect.sizeDelta.x, collapsedHeight);
        var c = targetImage.color;
        c.a = collapsedAlpha;
        targetImage.color = c;
        optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        isFirstOpen = true;
    }

    public void StartExpand()
    {
        if (tweenCoroutine != null) StopCoroutine(tweenCoroutine);
        tweenCoroutine = StartCoroutine(ExpandThenShow());
    }

    public void StartCollapse()
    {
        if (tweenCoroutine != null) StopCoroutine(tweenCoroutine);
        tweenCoroutine = StartCoroutine(CollapseThenReturn());
    }

    public void ResetPanels()
    {
        foreach (var sv in optionScrollViews)
            sv.SetActive(false);
        isFirstOpen = true;
    }

    public IEnumerator ExpandThenShow()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);

        yield return TweenUI(expandedHeight, expandedAlpha);

        foreach (var sv in optionScrollViews)
            sv.SetActive(false);

        isFirstOpen = false;
        tweenCoroutine = null;
    }

    public IEnumerator CollapseThenReturn()
    {
        foreach (var sv in optionScrollViews)
            sv.SetActive(false);

        optionsMenuUI.SetActive(false);

        yield return TweenUI(collapsedHeight, collapsedAlpha);

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Background.SetActive(false);
            optionsMenuUI.SetActive(false);
        }

        isFirstOpen = true;
        tweenCoroutine = null;
    }

    private IEnumerator TweenUI(float targetH, float targetA)
    {
        float startH = targetRect.sizeDelta.y;
        float startA = targetImage.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // always tween height
            targetRect.sizeDelta = new Vector2(
                targetRect.sizeDelta.x,
                Mathf.Lerp(startH, targetH, t)
            );

            // only tween alpha on non-main-menu scenes
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                Color c = targetImage.color;
                c.a = Mathf.Lerp(startA, targetA, t);
                targetImage.color = c;
            }

            yield return null;
        }

        // finalize height
        targetRect.sizeDelta = new Vector2(targetRect.sizeDelta.x, targetH);

        // finalize alpha only on non-main-menu scenes
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Color c = targetImage.color;
            c.a = targetA;
            targetImage.color = c;
        }
    }

}
