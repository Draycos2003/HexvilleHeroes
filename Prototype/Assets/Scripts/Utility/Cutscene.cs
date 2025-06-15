using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("Cutscene") || sceneName.Contains("Intro"))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
