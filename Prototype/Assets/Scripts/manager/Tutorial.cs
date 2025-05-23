using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] string text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gamemanager.instance.textPopupDesc.text = text;
            gamemanager.instance.textPopup.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gamemanager.instance.textPopup.SetActive(false);
        }
    }
}