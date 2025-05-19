using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [Header("Menues")]
    [SerializeField] GameObject MenuActive;
    [SerializeField] GameObject MenuPaused;
    [SerializeField] GameObject MenuWin;
    [SerializeField] GameObject MenuLose;
    [SerializeField] GameObject MenuInventory;

    [Header("Match Timer")]
    [SerializeField] TMP_Text winMessageText;
    private float matchTime;
    private bool matchEnded;

    [Header("Player Objects")]
    public GameObject playerDMGScreen;
    public GameObject playerShieldDMGScreen;
    public GameObject Player;
    public playerController PlayerScript;

    public bool isPaused;

    public pickupItem pickUp;

    [Header("Enemy Info")]
    [SerializeField] TMP_Text gameGoalCountText;

    float timeScaleOrig;
    int gameGoalCount;

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
            else if (MenuActive == MenuPaused || MenuActive == MenuInventory)
            {
                stateUnpause();
            }
        }

        openInventory();

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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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
                winMessageText.text = "You successfully beat the level in " + MatchTime() + "!";
            }
        }
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

    public void openInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (MenuActive == null)
            {
                statePause();
                MenuActive = MenuInventory;
                MenuActive.SetActive(isPaused);
            }
            else if (MenuActive == MenuInventory)
            {
                stateUnpause();
            }
        }
    }
}