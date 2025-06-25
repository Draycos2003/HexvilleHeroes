using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    [Header("Scene Loader (Build-index # or Name)")]
    [Tooltip("Enter a scene build-index (e.g. '2') or scene name (e.g. 'Level 2').")]
    [SerializeField] private string Scene;

    private ThirdPersonCamController camCtrl;
    private VideoPlayer videoPlayer;

    private void Start()
    {
        camCtrl = FindFirstObjectByType<ThirdPersonCamController>();
        if (camCtrl != null)
            camCtrl.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
        else
        {
            Debug.LogWarning("CutsceneManager: No VideoPlayer found. Video end won't auto-skip.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipCutscene();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SkipCutscene();
    }

    private void SkipCutscene()
    {
        if (camCtrl != null) camCtrl.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
            Debug.LogError("CutsceneManager: Scene field is empty or invalid!");
        }
    }
}
