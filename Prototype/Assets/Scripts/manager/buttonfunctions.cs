using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonfunctions : MonoBehaviour
{
    private GameObject Player;

    void Start()
    {
        Player = gamemanager.instance?.Player;

        if (Player == null)
        {
            Debug.LogError("[Currency] Player not found from GameManager.");
        }
    }

    private playerController playerControl;

    public void Resume()
    {
        gamemanager.instance.stateUnpause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();

        playerController pc = Player.GetComponent<playerController>();
        if (pc != null)
        {
            //Debug.Log($"[ButtonFunctions] HP before reset: {pc.HP}, Shield before reset: {pc.Shield}");
            //Debug.Log($"[ButtonFunctions] HPOrig: {pc.MAXHPOrig}, ShieldOrig: {pc.MAXShieldOrig}");

            pc.HP = pc.MAXHPOrig;
            pc.Shield = pc.MAXShieldOrig;

            //Debug.Log($"[ButtonFunctions] HP after reset: {pc.HP}, Shield after reset: {pc.Shield}");
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void loadLevel(int Lvl)
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        var gm = gamemanager.instance;

        if (currentScene == 1)
        {
            if (gm != null && gm.PlayerScript != null)
            {
                int originalScene = gm.PlayerScript.GetOriginalSceneIndex();
                //Debug.Log("[ButtonFunctions] Returning to original scene: " + originalScene);
                SceneManager.LoadScene(originalScene);
            }
        }
        else
        {
            //Debug.Log("[ButtonFunctions] Loading scene: " + Lvl);
            SceneManager.LoadScene(Lvl);
        }

        gm.stateUnpause();
    }

    public void ReturnToOriginalScene()
    {
        var gm = gamemanager.instance;

        if (gm != null && gm.PlayerScript != null)
        {
            int originalScene = gm.PlayerScript.GetOriginalSceneIndex();
            SceneManager.LoadScene(originalScene);
            gm.stateUnpause();
        }
    }

    public void onClickOptions()
    {
        var gm = gamemanager.instance;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (gm.MenuOptions != null && gm.PauseMenu != null)
        {
            bool optionsActive = gm.MenuOptions.activeSelf;

            if (!optionsActive)
            {
                gm.PauseMenu.SetActive(false);
                gm.setActiveMenu(gm.MenuOptions);
            }
            else
            {
                gm.MenuOptions.SetActive(false);

                if (sceneIndex != 0)
                {
                    gm.setActiveMenu(gm.PauseMenu);
                }
                else
                {
                    gm.MenuActive = null;
                    gm.stateUnpause();
                }
            }
        }
    }

    public void onLoadGame()
    {
        playerControl = gamemanager.instance.PlayerScript;
        if (playerControl.playerMenu != null)
        {
            playerControl.playerMenu.SetActive(true);
        }

        gamemanager.instance.Load();
    }

    public void onEquip()
    {

    }

    public void onUse()
    {

    }

    public void onDrop()
    {

    }
}