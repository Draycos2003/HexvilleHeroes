using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class LoadHandler : MonoBehaviour
{
    [Header("Scenes to disable controls & UI in")]
    [SerializeField] private string[] disableScenes = new string[] { };

    [Header("Teleport on Load")]
    [SerializeField] private string teleportTargetName = "SpawnPoint";

    private bool shouldRestoreSave = false;
    private SaveData pendingSaveData;

    private playerController pc;
    private ThirdPersonCamController camCtrl;

    private void Awake()
    {
        pc = FindFirstObjectByType<playerController>();
        camCtrl = FindFirstObjectByType<ThirdPersonCamController>();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    public void QueueRestore(SaveData data)
    {
        pendingSaveData = data;
        shouldRestoreSave = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool disable = disableScenes.Contains(scene.name);

        // toggle controls & UI
        if (pc != null) pc.enabled = !disable;
        if (camCtrl != null) camCtrl.enabled = !disable;
        if (pc != null)
        {
            pc.inventoryCanvas?.SetActive(!disable);
            pc.playerMenu?.SetActive(!disable);
        }

        var playerGO = gamemanager.instance?.Player;
        if (playerGO == null) return;

        if (shouldRestoreSave)
        {
            // Restore from save
            var cc = playerGO.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            playerGO.transform.position = new Vector3(
                pendingSaveData.playerX,
                pendingSaveData.playerY,
                pendingSaveData.playerZ
            );

            if (cc) cc.enabled = true;

            var playerCtr = playerGO.GetComponent<playerController>();
            if (playerCtr != null)
            {
                // health & shield
                playerCtr.HP = pendingSaveData.playerHP;
                playerCtr.Shield = pendingSaveData.playerShield;

                // inventory (null-guarded)
                var invList = playerCtr.weaponAgent
                                ?.inventoryData
                                ?.inventoryItems;
                var savedItems = pendingSaveData.inventory;
                if (invList != null && savedItems != null)
                {
                    for (int i = 0; i < savedItems.Count && i < invList.Count; i++)
                        invList[i] = savedItems[i];
                }
                else
                {
                    Debug.LogWarning("[LoadHandler] inventoryData/items null - skipping inventory restore.");
                }

                // refresh UI
                playerCtr.inventoryCanvas?.SetActive(true);
                playerCtr.playerMenu?.SetActive(true);
            }

            shouldRestoreSave = false;
            pendingSaveData = null;
        }
        else
        {
            // Fallback to SpawnPoint
            var spawn = GameObject.Find(teleportTargetName);
            if (spawn != null)
            {
                var cc2 = playerGO.GetComponent<CharacterController>();
                if (cc2) cc2.enabled = false;

                playerGO.transform.SetPositionAndRotation(
                    spawn.transform.position,
                    spawn.transform.rotation
                );

                if (cc2) cc2.enabled = true;
            }
        }
    }
}
