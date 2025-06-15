using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class LoadHandler : MonoBehaviour
{
    [Header("Scenes to disable controls & UI in")]
    [Tooltip("Exact scene names where playerControl, camera, and UI stay off")]
    [SerializeField] private string[] disableScenes = new string[] {};

    private playerController pc;
    private ThirdPersonCamController camCtrl;

    private void Awake()
    {
        pc = FindFirstObjectByType<playerController>();
        if (pc == null)
            Debug.LogWarning($"[{nameof(LoadHandler)}] No playerController found in scene");

        camCtrl = FindFirstObjectByType<ThirdPersonCamController>();
        if (camCtrl == null)
            Debug.LogWarning($"[{nameof(LoadHandler)}] No ThirdPersonCamController found in scene");
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool disable = disableScenes.Contains(scene.name);

        // toggle playerController & camera
        if (pc != null) pc.enabled = !disable;
        if (camCtrl != null) camCtrl.enabled = !disable;

        // only show inventory & menu in enabled scenes
        if (pc != null)
        {
            if (pc.inventoryCanvas != null)
                pc.inventoryCanvas.SetActive(!disable);
            if (pc.playerMenu != null)
                pc.playerMenu.SetActive(!disable);
        }
    }
}
