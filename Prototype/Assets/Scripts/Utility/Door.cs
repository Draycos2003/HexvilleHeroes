using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject doorModel;
    [SerializeField] GameObject button;
    [SerializeField] string Text;

    bool playerintriggr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerintriggr)
        {
            if (Input.GetButtonDown("Interact"))
            {
                doorModel.SetActive(false);
                button.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IOpen openable = other.GetComponent<IOpen>();

        if (openable != null)
        {
            playerintriggr = true;
            gamemanager.instance.PopupText.text = Text;
            gamemanager.instance.TextPopup.SetActive(true);

            button.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IOpen openable = other.GetComponent<IOpen>();

        if (openable != null)
        {
            playerintriggr = false;
            button.SetActive(false);
            doorModel.SetActive(true);

            gamemanager.instance.TextPopup.SetActive(false);
        }
    }
}
