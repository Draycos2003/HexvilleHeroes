using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CommandConsole : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject commandBarUI;
    public GameObject secondaryUI;

    [Header("Input Field")]
    public TMP_InputField inputField;

    [Header("Output Display")]
    public GameObject outputHolderUI;
    public RectTransform outputHolderRect;
    public TMP_Text outputText;
    public float outputPadding = 10f;

    [Header("Disable While Console Open")]
    public List<Behaviour> componentsToDisable = new();
    public bool autoDetectPlayerController = true;

    [Header("Offsets")]
    public float holderYOffset = 15f;
    public float textYOffset = 5f;

    private CommandHandler handler;
    private bool isOpen;
    private float originalHolderHeight;
    private float topInset;
    private float originalTextY;
    private List<Behaviour> disableList;

    void Start()
    {
        handler = new CommandHandler(AppendOutput);

        // cache sizes & positions (I hate this crap lol)
        originalHolderHeight = outputHolderRect.rect.height;
        topInset = -outputHolderRect.offsetMax.y + holderYOffset;
        originalTextY = outputText.rectTransform.anchoredPosition.y;
        outputText.rectTransform.anchoredPosition =
            new Vector2(outputText.rectTransform.anchoredPosition.x,
                        originalTextY + textYOffset);

        // build the list of components to disable
        disableList = new List<Behaviour>(componentsToDisable);

        if (autoDetectPlayerController)
        {
            var playerGO = GameObject.FindWithTag("Player");
            if (playerGO == null)
            {
                Debug.LogWarning("CommandConsole: no GameObject tagged 'Player' found.");
            }
            else
            {
                // generic, type-safe lookup
                var pc = playerGO.GetComponent<playerController>();
                if (pc == null)
                    Debug.LogWarning("CommandConsole: 'Player' has no PlayerController component.");
                else
                {
                    disableList.Add(pc);
                    //Debug.Log("CommandConsole: auto-added PlayerController to disableList.");
                }
            }
        }

        // initial hide
        commandBarUI.SetActive(false);
        secondaryUI.SetActive(false);
        outputHolderUI.SetActive(false);

        // filter out backquotes in the input field
        inputField.characterValidation = TMP_InputField.CharacterValidation.None;
        inputField.onValidateInput += (s, i, c) => (c == '`' || c == '~') ? '\0' : c;

        outputText.alignment = TextAlignmentOptions.TopLeft;
        outputText.overflowMode = TextOverflowModes.Overflow;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            ToggleConsole();

        if (isOpen && Input.GetKeyDown(KeyCode.Return))
        {
            handler.Execute(inputField.text);
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    void ToggleConsole()
    {
        isOpen = !isOpen;
        commandBarUI.SetActive(isOpen);
        secondaryUI.SetActive(isOpen);

        foreach (var comp in disableList)
            if (comp != null)
                comp.enabled = !isOpen;

        if (isOpen)
        {
            outputText.text = "";
            outputHolderUI.SetActive(false);
            SetHolderHeight(originalHolderHeight);
            inputField.ActivateInputField();
        }
    }

    public void AppendOutput(string message)
    {
        if (!outputHolderUI.activeSelf)
            outputHolderUI.SetActive(true);

        outputText.text += message + "\n";
        outputText.ForceMeshUpdate();

        float width = outputHolderRect.rect.width;
        Vector2 prefValues = outputText.GetPreferredValues(outputText.text, width, Mathf.Infinity);
        float h = Mathf.Max(originalHolderHeight, prefValues.y + outputPadding);
        SetHolderHeight(h);
    }

    void SetHolderHeight(float height)
    {
        outputHolderRect.SetInsetAndSizeFromParentEdge(
            RectTransform.Edge.Top,
            topInset,
            height
        );
    }
}
