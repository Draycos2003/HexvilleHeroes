using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockYmain, lockYmin;
    [SerializeField] bool InvertY;

    float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        if (InvertY)
        {
            rotX -= mouseY;
        }
        else
            rotX += mouseY;

    }
}
