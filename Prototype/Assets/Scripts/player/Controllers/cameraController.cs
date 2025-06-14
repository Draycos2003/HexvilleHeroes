using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Input
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        // Option to Invert (Look Up and Down)
        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        // Clamp Camera on x-Axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // Rotate the Camera on X Axis to look up and down
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // Rotate the Player on Y Axis to look left and right
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
