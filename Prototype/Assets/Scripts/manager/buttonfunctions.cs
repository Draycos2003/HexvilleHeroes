using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonfunctions : MonoBehaviour
{
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
}
