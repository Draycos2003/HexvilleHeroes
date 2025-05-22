using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonfunctions : MonoBehaviour
{

    private static readonly string teleportTargetName = "SpawnPoint";

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
        if (target != null)
        {
            Transform player = gamemanager.instance.Player.transform;

            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.position = target.transform.position;
            player.rotation = target.transform.rotation;

            if (cc != null) cc.enabled = true;

            Debug.Log("[ButtonFunctions] Teleported to: " + teleportTargetName);
        }
        else
        {
            Debug.LogWarning("[ButtonFunctions] SpawnPoint not found.");
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
                Debug.Log("[ButtonFunctions] Returning to original scene: " + originalScene);
                SceneManager.LoadScene(originalScene);
            }
        }
        else
        {
            Debug.Log("[ButtonFunctions] Loading scene: " + Lvl);
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
}