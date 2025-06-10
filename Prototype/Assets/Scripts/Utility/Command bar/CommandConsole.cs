using UnityEngine;
using TMPro;

public class CommandConsole : MonoBehaviour
{
    public GameObject commandBarUI;
    public GameObject terminalUI;
    public TMP_InputField inputField;

    private float lastTildeTime = 0f;
    private float doublePressThreshold = 0.3f;
    private bool isCommandBarOpen = false;

    private CommandHandler commandHandler;

    void Start()
    {
        commandHandler = new CommandHandler();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            float timeSinceLast = Time.time - lastTildeTime;

            if (timeSinceLast <= doublePressThreshold)
            {
                ToggleTerminal();
                lastTildeTime = 0f;
            }
            else
            {
                ToggleCommandBar();
                lastTildeTime = Time.time;
            }
        }

        if (isCommandBarOpen && Input.GetKeyDown(KeyCode.Return))
        {
            commandHandler.Execute(inputField.text);
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    void ToggleCommandBar()
    {
        isCommandBarOpen = !isCommandBarOpen;
        commandBarUI.SetActive(isCommandBarOpen);
        if (isCommandBarOpen)
            inputField.ActivateInputField();
    }

    void ToggleTerminal()
    {
        terminalUI.SetActive(!terminalUI.activeSelf);
    }
}
