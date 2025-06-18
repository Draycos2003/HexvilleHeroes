using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using System.Collections.Generic;

public class gamemanager : MonoBehaviour
{
    #region Singleton
    public static gamemanager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
        timeScaleOrig = Time.timeScale;
        CachePlayer();
        InitializeUI();
    }
    #endregion

    #region Serialized Fields
    [Header("Menus & Overlays")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject loseMenu;
    [SerializeField] private GameObject backgroundShade;
    [SerializeField] private GameObject[] hideOnPauseMenus;

    [Header("Tween Handlers")]
    [SerializeField] private RectHeightAndAlphaTweener rectTweener;
    [SerializeField] private ViewerPanelTween viewerTween;

    [Header("Post-Processing")]
    [SerializeField] private PostProcessVolume postProcessVolume;

    [Header("Match Timer")]
    [SerializeField] private TMP_Text winMessageText;

    [Header("Enemy Goals")]
    [SerializeField] private TMP_Text gameGoalCountText;

    [Header("Player Screens")]
    [SerializeField] public GameObject playerDMGScreen;
    [SerializeField] public GameObject playerShieldDMGScreen;

    [Header("Popup")]
    [SerializeField] public GameObject TextPopup;
    [SerializeField] public TMP_Text PopupText;

    [Header("Inventory UI")]
    [SerializeField] private inventoryUI invUI;
    [SerializeField] private inventoryController invController;
    [SerializeField] private inventorySO saveInv;
    #endregion

    #region State Fields
    private float timeScaleOrig;
    private float matchTime;
    private bool matchEnded;
    private int gameGoalCount;
    private GameObject player;
    private playerController playerScript;
    private GameObject activeMenu;
    #endregion

    #region Properties
    public bool IsPaused => pauseMenu.activeSelf;
    public int GameGoalCount => gameGoalCount;
    public GameObject Player => player;
    public playerController PlayerScript => playerScript;
    public GameObject PauseMenu => pauseMenu;
    public GameObject MenuOptions => optionsMenu;
    public GameObject BackgroundShade => backgroundShade;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (!IsPaused && !matchEnded) matchTime += Time.deltaTime;
        if (Input.GetButtonDown("Cancel")) HandleEscape();
        if (Input.GetKeyDown(KeyCode.F5)) Save();
        if (Input.GetKeyUp(KeyCode.F6)) Load();
    }
    #endregion

    #region Scene Management
    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        matchTime = 0f;
        matchEnded = false;
        gameGoalCount = 0;
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
        gameGoalCountText.text = "0";
        CachePlayer();
    }

    private void CachePlayer()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player ? player.GetComponent<playerController>() : null;

        if (player != null)
            DontDestroyOnLoad(player);

        if (playerScript != null)
        {
            var list = new List<GameObject>(hideOnPauseMenus);
            if (playerScript.playerMenu != null)
                list.Add(playerScript.playerMenu);
            if (playerScript.inventoryCanvas != null)
                list.Add(playerScript.inventoryCanvas);
            hideOnPauseMenus = list.ToArray();
        }
    }


    private void InitializeUI()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        winMenu.SetActive(false);
        loseMenu.SetActive(false);
        backgroundShade.SetActive(false);
        TextPopup.SetActive(false);
        playerDMGScreen.SetActive(false);
        playerShieldDMGScreen.SetActive(false);
        if (postProcessVolume != null) postProcessVolume.weight = 0f;
    }
    #endregion

    #region Escape / Pause Flow
    private void HandleEscape()
    {
#if UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.Escape)) return;
#endif
        if (optionsMenu.activeSelf)
        {
            viewerTween.ResetPanels();
            StartCoroutine(CollapseOptionsThenReturn());
        }
        else if (pauseMenu.activeSelf)
        {
            Unpause();
        }
        else
        {
            Pause(false);
        }
    }

    private IEnumerator CollapseOptionsThenReturn()
    {
        optionsMenu.SetActive(false);
        yield return StartCoroutine(rectTweener.CollapseThenReturn());
        pauseMenu.SetActive(true);
        activeMenu = pauseMenu;
    }


    private void Pause(bool Result = false)
    {
        if (Result != true)
        {
            foreach (var go in hideOnPauseMenus)
                go.SetActive(false);

            activeMenu = pauseMenu;
            pauseMenu.SetActive(true);

            if (SceneManager.GetActiveScene().buildIndex != 0)
            {

                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

            }

            backgroundShade.SetActive(true);
            if (postProcessVolume != null) postProcessVolume.weight = 1f;
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {

                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

            }
            if (postProcessVolume != null) postProcessVolume.weight = 1f;
        }
        
    }

    private void Unpause()
    {
        foreach (var go in hideOnPauseMenus)
            go.SetActive(true);

        optionsMenu.SetActive(false);

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            
            Time.timeScale = timeScaleOrig;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
        }

        backgroundShade.SetActive(false);
        if (postProcessVolume != null) postProcessVolume.weight = 0f;

        viewerTween.ResetPanels();
        rectTweener.CollapseInstant();
        pauseMenu.SetActive(false);
        activeMenu = null;
    }

    public void statePause() => Pause(false);
    public void stateUnpause() => Unpause();
    #endregion

    #region Win / Lose
    public void youLose()
    {
        if (matchEnded) return;
        matchEnded = true;
        Pause(true);
        loseMenu.SetActive(true);
    }

    public void updateGameGoal(int amt)
    {
        gameGoalCount += amt;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        if (gameGoalCount <= 0) TryWin();
    }

    private void TryWin()
    {
        var prog = player?.GetComponent<PlayerProgression>();
        if (prog != null && prog.HasCheckpoint(SceneManager.GetActiveScene().buildIndex - 1))
            Win();
    }

    private void Win()
    {
        if (matchEnded) return;
        matchEnded = true;
        Pause(true);
        winMenu.SetActive(true);
        if (winMessageText != null)
            winMessageText.text =
                $"You cleared this room in {FormatTime(matchTime)}!\n\nPlease proceed to the next room";
    }
    #endregion

    #region Save / Load
    public void Save()
    {
        if (playerScript == null) return;
        var data = new SaveData
        {
            playerX = player.transform.position.x,
            playerY = player.transform.position.y,
            playerZ = player.transform.position.z,
            playerHP = playerScript.HP,
            playerShield = playerScript.Shield,
            currentScene = SceneManager.GetActiveScene().name,
            inventory = playerScript.weaponAgent
                                      .inventoryData
                                      .SaveInventory(playerScript.weaponAgent.inventoryData.inventoryItems)
        };
        SaveSystem.SaveGame(data);
    }

    public void Load()
    {
        var data = SaveSystem.LoadGame();
        if (data == null) return;
        var loader = FindFirstObjectByType<LoadHandler>();
        loader?.QueueRestore(data);
        SceneManager.LoadScene(data.currentScene);
    }
    #endregion

    #region Utilities
    public void SetMatchTime(int t) => matchTime = t;

    public void ShowDamage() => playerDMGScreen.SetActive(true);
    public void HideDamage() => playerDMGScreen.SetActive(false);
    public void ShowShield() => playerShieldDMGScreen.SetActive(true);
    public void HideShield() => playerShieldDMGScreen.SetActive(false);

    public void ShowPopup(string msg)
    {
        if (PopupText != null) PopupText.text = msg;
        TextPopup.SetActive(true);
    }

    public void HidePopup() => TextPopup.SetActive(false);

    public void setActiveMenu(GameObject menu)
    {
        if (menu == optionsMenu)
        {
            activeMenu = optionsMenu;
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }
        else if (menu == pauseMenu)
        {
            activeMenu = pauseMenu;
            optionsMenu.SetActive(false);
            pauseMenu.SetActive(true);
        }
        else
        {
            if (activeMenu != null) activeMenu.SetActive(false);
            activeMenu = menu;
            activeMenu.SetActive(true);
        }
    }

    private string FormatTime(float t)
    {
        int ts = Mathf.FloorToInt(t), h = ts / 3600, m = (ts % 3600) / 60, s = ts % 60;
        return $"{h}:{m:00}:{s:00}";
    }
    #endregion
}
