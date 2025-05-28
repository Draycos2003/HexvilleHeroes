using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam != null)
        {
            Vector3 toCam = mainCam.transform.position - transform.position;

            transform.rotation = Quaternion.LookRotation(-toCam, Vector3.up);
        }
    }
}
