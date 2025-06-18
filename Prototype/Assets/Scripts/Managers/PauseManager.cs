using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button quitGameButton;
    private bool isPaused = false;

#if UNITY_WEBGL
    private KeyCode pauseKey = KeyCode.P;
#endif
    void Start()
    {
#if UNITY_WEBGL
        // Hide the Quit button in WebGL builds
        if (quitGameButton != null)
            quitGameButton.gameObject.SetActive(false);
#endif

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
#if UNITY_WEBGL
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
#endif
    }

    public void TogglePause()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = isPaused ? 0 : 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            isPaused = false;
        }

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(isPaused);
    }

    public void QuitGame()
    {
#if !UNITY_WEBGL
        Application.Quit();
#endif
    }
}