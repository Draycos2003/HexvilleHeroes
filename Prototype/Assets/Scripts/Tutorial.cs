using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject TutorialCanvas;
    [SerializeField] TMP_Text TutorialText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialCanvas.SetActive(true);
            //TutorialText.text = "Text youd want to change";

            // Below is wrong, unless you want a tutorial object in every level? We can just have the tutorial be a prefab and place it where we need it, then change it however we need
            //gamemanager.instance.textPopupDesc.text = text;
            //gamemanager.instance.textPopup.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialCanvas.SetActive(false);
        }
    }
}