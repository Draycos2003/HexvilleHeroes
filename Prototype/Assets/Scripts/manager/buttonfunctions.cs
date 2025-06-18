using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonFunctions : MonoBehaviour
{
    private gamemanager gm => gamemanager.instance;
    private GameObject player => gm?.Player;
    private playerController playerController => gm?.PlayerScript;

    private void Awake()
    {
        if (player == null)
            Debug.LogError("[ButtonFunctions] Player not found in GameManager.");
    }

    public void Resume()
    {
        gm.stateUnpause();
    }

    public void Restart()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
        gm.stateUnpause();

        if (playerController != null)
        {
            playerController.HP = playerController.MAXHPOrig;
            playerController.Shield = playerController.MAXShieldOrig;
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
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex == 1 && playerController != null)
        {
            int original = playerController.GetOriginalSceneIndex();
            SceneManager.LoadScene(original);
        }
        else
        {
            SceneManager.LoadScene(levelIndex);
        }
        gm.stateUnpause();
    }

    public void ReturnToOriginalScene()
    {
        if (playerController != null)
        {
            int original = playerController.GetOriginalSceneIndex();
            SceneManager.LoadScene(original);
            gm.stateUnpause();
        }
    }

    public void ToggleOptions()
    {
        if (gm.MenuOptions == null || gm.PauseMenu == null) return;

        bool optionsActive = gm.MenuOptions.activeSelf;
        if (!optionsActive)
        {
            gm.PauseMenu.SetActive(false);
            gm.setActiveMenu(gm.MenuOptions);
        }
        else
        {
            gm.MenuOptions.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex != 0)
                gm.setActiveMenu(gm.PauseMenu);
            else
            {
                gm.MenuActive = null;
                gm.stateUnpause();
            }
        }
    }

    public void LoadGame()
    {
        playerController?.playerMenu?.SetActive(true);
        gm.Load();
    }

    public void OnEquip() { }
    public void OnUse() { }
    public void OnDrop() { }
}
