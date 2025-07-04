using FinalController;
using UnityEngine;

public class ThirdPersonCamController : MonoBehaviour
{
    [Header("Camera Follow")]
    public Transform targetTransform; // object the camera will follow
    private Vector3 camerFollowVel = Vector3.zero;

    public float cameraFollowSpeed = 0.2f;

    public float mouseSensitivity = 100.0f;
    public float lookSensH = 0.1f;
    public float lookSensV = 0.1f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    [Header("Player Look Direction")]
    [SerializeField] private playerInput PLI;


    private void Awake()
    {
        targetTransform = FindFirstObjectByType<playerController>().transform;
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    public void FollowTarget()
    {
        Vector3 targetPos = Vector3.SmoothDamp
            (transform.position, targetTransform.position, ref camerFollowVel, cameraFollowSpeed);

        transform.position = targetPos;
    }

    void LateUpdate()
    {
        float mouseX = PLI.lookInput.x;
        float mouseY = -PLI.lookInput.y;

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;

    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotY, 0);
}
   

