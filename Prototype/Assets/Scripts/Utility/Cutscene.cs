using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Scene Loader (Build-index # or Name)")]
    [Tooltip("Enter a scene build-index (e.g. “2”) or scene name (e.g. “Level2”).")]
    [SerializeField] private string Scene;

    private ThirdPersonCamController camCtrl;

    private void Start()
    {
        camCtrl = FindFirstObjectByType<ThirdPersonCamController>();
        if (camCtrl != null)
            camCtrl.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            // restore camera & cursor
            if (camCtrl != null) camCtrl.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // try to parse as build-index, else treat as scene name
            if (int.TryParse(Scene, out int sceneIndex) && sceneIndex >= 0)
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else if (!string.IsNullOrWhiteSpace(Scene))
            {
                SceneManager.LoadScene(Scene);
            }
            else
            {
                Debug.LogError("CutsceneManager: Hotkey Scene field is empty or invalid!");
            }
        }
    }
}
