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

    private static readonly string teleportTargetName = "SpawnPoint";
    private playerController playerControl;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject target = GameObject.Find(teleportTargetName);
        
        if (target != null && gamemanager.instance.Player != null)
        {
            Transform player = gamemanager.instance.Player.transform;

            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.position = target.transform.position;
            player.rotation = target.transform.rotation;

            if (cc != null) cc.enabled = true;

            //Debug.Log("[ButtonFunctions] Teleported to: " + teleportTargetName);
        }
        else
        {
            //Debug.LogWarning("[ButtonFunctions] SpawnPoint not found.");
        }
    }

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

        playerControl = gamemanager.instance.PlayerScript;
        if(playerControl.inventoryCanvas != null)
        {
            playerControl.inventoryCanvas.SetActive(true);
        }

        if (playerControl.playerMenu != null)
        {
            playerControl.playerMenu.SetActive(true);
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
        gamemanager.instance.setActiveMenu(gamemanager.instance.OptionsMenu);
    }
}