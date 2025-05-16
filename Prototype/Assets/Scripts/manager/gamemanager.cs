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

    [Header("Player Objects")]
    public GameObject playerDMGScreen;
    public GameObject Player;
    public playerController PlayerScript;

    public bool isPaused;

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
        PlayerScript = Player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
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

        MenuActive.SetActive(false);
        MenuActive = null;
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
            statePause();
            MenuActive = MenuWin;
            MenuActive.SetActive(true);
        }
    }
}