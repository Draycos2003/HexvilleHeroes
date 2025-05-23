using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] inventoryUI invUI;

    public int inventorySize;

    public void Start()
    {
        invUI.InitializeInventoryUI(inventorySize);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if(invUI.isActiveAndEnabled == false)
            {
                invUI.Show();
                gamemanager.instance.statePause();
            }
            else
            {
                invUI.Hide();
                gamemanager.instance.stateUnpause();
            }
        }


    }
}
