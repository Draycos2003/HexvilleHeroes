using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ViewerPanelTween : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        public Button button;
        public RectTransform panel;
        [HideInInspector] public Vector2 visiblePos;
        [HideInInspector] public float startX;
    }

    [Header("Button : Panel pairs")]
    public List<Entry> entries = new List<Entry>();

    [Header("Back Button")]
    [SerializeField] private Button resetButton;

    [Header("Tween Settings")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float offsetFactor = 1f;

    [Header("Hook in your Tweener here!")]
    [SerializeField] private RectHeightAndAlphaTweener rectTweener;

    private int currentIndex = -1;
    private Coroutine tweenRoutine;

    private void Awake()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            e.visiblePos = e.panel.anchoredPosition;
            e.startX = e.visiblePos.x;
            e.panel.gameObject.SetActive(false);

            int idx = i;
            e.button.onClick.AddListener(() =>
            {
                ShowPanel(idx);
                if (rectTweener != null) rectTweener.isFirstOpen = false;
            });
        }

        if (resetButton != null)
            resetButton.onClick.AddListener(() =>
            {
                ResetPanels();
                if (rectTweener != null) rectTweener.isFirstOpen = true;
            });
    }

    public void ResetPanels()
    {
        if (tweenRoutine != null)
        {
            StopCoroutine(tweenRoutine);
            tweenRoutine = null;
        }

        foreach (var e in entries)
            e.panel.gameObject.SetActive(false);

        currentIndex = -1;
    }

    public void ShowPanel(int newIndex)
    {
        if (newIndex == currentIndex) return;

        if (tweenRoutine != null)
        {
            StopCoroutine(tweenRoutine);
            tweenRoutine = null;
        }

        tweenRoutine = StartCoroutine(SwitchPanels(newIndex));
    }

    private IEnumerator SwitchPanels(int newIndex)
    {
        if (currentIndex < 0)
        {
            var first = entries[newIndex];
            first.panel.gameObject.SetActive(true);
            first.panel.anchoredPosition = first.visiblePos;
            currentIndex = newIndex;
            yield break;
        }

        var curr = entries[currentIndex];
        var up = new Vector2(curr.startX,
                               curr.visiblePos.y + curr.panel.rect.height * offsetFactor);
        yield return Tween(curr.panel, curr.visiblePos, up);
        curr.panel.gameObject.SetActive(false);

        var next = entries[newIndex];
        var startDown = new Vector2(next.startX,
                                    next.visiblePos.y - next.panel.rect.height * offsetFactor);
        next.panel.gameObject.SetActive(true);
        next.panel.anchoredPosition = startDown;
        yield return Tween(next.panel, startDown, next.visiblePos);

        currentIndex = newIndex;
        tweenRoutine = null;
    }

    private IEnumerator Tween(RectTransform rt, Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            rt.anchoredPosition = Vector2.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        rt.anchoredPosition = to;
    }
}
