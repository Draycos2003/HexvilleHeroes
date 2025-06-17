using UnityEngine;
using UnityEngine.UI;

public class UGUICreditsRoller : MonoBehaviour
{
    [Header("UI References")]
    public GameObject creditsPanel;
    public CreditsData creditsData;
    public GameObject entryPrefab;
    public ScrollRect scrollRect;
    public RectTransform contentContainer;

    [Header("Scroll Settings")]
    public float scrollSpeed = 50f;

    float targetY;
    bool isRolling;

    public void StartCredits()
    {
        creditsPanel.SetActive(true);

        foreach (Transform t in contentContainer) Destroy(t.gameObject);

        foreach (var line in creditsData.lines)
        {
            var go = Instantiate(entryPrefab, contentContainer);
            var entry = go.GetComponent<CreditEntry>();
            entry.nameText.text = line.name;
            entry.roleText.text = line.role;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainer);

        scrollRect.enabled = false;

        float contentHeight = contentContainer.rect.height;
        contentContainer.anchoredPosition = new Vector2(0, -contentHeight);

        targetY = contentHeight + Screen.height;
        isRolling = true;
    }

    void Update()
    {
        if (!isRolling) return;

        // move up
        contentContainer.anchoredPosition += Vector2.up * (scrollSpeed * Time.deltaTime);

        // stop when it's fully off the top (>= targetY)
        if (contentContainer.anchoredPosition.y >= targetY)
        {
            isRolling = false;
            EndCredits();
        }
    }

    private void EndCredits()
    {
        // hide the Credits UI
        creditsPanel.SetActive(false);
        foreach (Transform t in contentContainer) Destroy(t.gameObject);
        scrollRect.enabled = true;
    }

    public void StopCredits()
    {
        isRolling = false;
        creditsPanel.SetActive(false);        
        contentContainer.anchoredPosition = Vector2.zero;  // reset 

        foreach (Transform t in contentContainer) Destroy(t.gameObject);
    }

}
