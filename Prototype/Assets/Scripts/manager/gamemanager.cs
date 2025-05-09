using UnityEngine;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject MenuActive;
    [SerializeField] GameObject MenuPaused;
    [SerializeField] GameObject MenuWin;
    [SerializeField] GameObject MenuLose;

    public GameObject Player;
    public playerController PlayerScript;

    public bool isPaused;

    float timeScaleOrig;

    int gameGoalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        Player = GameObject.FindWithTag("Player");
        PlayerScript = Player.GetComponent<playerController>();
        timeScaleOrig = Time.timeScale;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if(MenuActive == null)
            {
                statePause();
                MenuActive = MenuPaused;
                MenuActive.SetActive(isPaused);
            }
            else if(MenuActive == MenuPaused)
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

        if (gameGoalCount <= 0)
        {
            statePause();
            MenuActive = MenuWin;
            MenuActive.SetActive(true);
        }
    }
}