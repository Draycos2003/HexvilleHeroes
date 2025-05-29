using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [Header("Menues")]
    [SerializeField] public GameObject MenuActive;
    [SerializeField] GameObject MenuPaused;
    [SerializeField] GameObject MenuWin;
    [SerializeField] GameObject MenuLose;
    [SerializeField] GameObject OptionsMenu;
    public GameObject PauseMenu => MenuPaused;
    public GameObject MenuOptions => OptionsMenu;

    [Header("Match Timer")]
    [SerializeField] TMP_Text winMessageText;
    private float matchTime;
    private bool matchEnded;

    [Header("Player Objects")]
    public GameObject playerDMGScreen;
    public GameObject playerShieldDMGScreen;
    public GameObject Player;
    public playerController PlayerScript;

    public GameObject TextPopup;
    public TMP_Text PopupText;

    public bool isPaused;

    public collectiblePickup pickUp;

    [Header("Enemy Info")]
    [SerializeField] TMP_Text gameGoalCountText;

    [Header("InventoryUI")]
    [SerializeField] inventoryUI invUI;
    [SerializeField] inventoryController invController;

    float timeScaleOrig;
    public int gameGoalCount;

    public int GameGoalCount => gameGoalCount;

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
        GameObject player = gamemanager.instance.Player;

        if (player == null)
        {
            Debug.LogWarning("[SceneEnemyBinder] Player not found.");
            return;
        }

        Transform playerTransform = player.transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        instance = this;
        Player = GameObject.FindWithTag("Player");
        timeScaleOrig = Time.timeScale;

        if (currentSceneIndex != 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }

    void Start()
    {
        DontDestroyOnLoad(Player);
        PlayerScript = Player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (!isPaused && !matchEnded)
            {
                matchTime += Time.deltaTime;
            }

            if (Input.GetButtonDown("Cancel"))
            {
                if (MenuActive == null)
                {
                    statePause();
                    MenuActive = MenuPaused;
                    MenuActive.SetActive(isPaused);
                }
                else if (MenuActive == MenuPaused)
                {
                    stateUnpause();
                }
            }
        }
    }


    public void statePause()
    {
        isPaused = !isPaused;

        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;

        Time.timeScale = timeScaleOrig;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
           
        if(MenuActive != null)
        {
            MenuActive.SetActive(false);
            MenuActive = null;
        }
    }

    public void youLose()
    {
        statePause();
        MenuActive = MenuLose;
        MenuActive.SetActive(true);
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        if (gameGoalCount <= 0)
        {
            matchEnded = true;
            statePause();
            MenuActive = MenuWin;
            MenuActive.SetActive(true);

            // update win text
            if (winMessageText != null)
            {
                winMessageText.text = "You have clear this room in " + MatchTime() + "!\n\nPlease proceed to the next room";
            }
        }
    }

    public void SetMatchTime(int time)
    {
        matchTime = time;
    }

    // calculate match time for hours minutes and seconds
    private string MatchTime()
    {
        int totalSeconds = Mathf.FloorToInt(matchTime);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        return string.Format("{0}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    public void setActiveMenu(GameObject menu)
    {
        if (MenuActive == null)
        {
            statePause();
            MenuActive = menu;
            MenuActive.SetActive(isPaused);
        }
        else if (MenuActive == menu)
        {
            stateUnpause();
        }
    }
}