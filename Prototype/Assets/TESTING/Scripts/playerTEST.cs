using NUnit.Framework.Internal;
using System.Collections;
using UnityEngine;

public class playerTEST : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int sprintMod;
    [SerializeField] private int jumpMax;
    [SerializeField] private int jumpForce;

    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip[] steps;
    [SerializeField] float stepVol;

    private Vector3 moveDir;
    private Vector3 playerVel;
    private bool isSprinting;
    private int jumpCount;
    private float shootTimer;
    private bool isPlayingStep;

    [SerializeField] private int gravity;


    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private float animTransSpeed;
    public bool isInteracting;

    Quaternion targetRotation;
    private ThirdPersonCamController thirdPerson;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thirdPerson = FindFirstObjectByType<ThirdPersonCamController>();
        animator = GetComponent<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
        Sprint();
        UpdateAnimValues();
    }

    private void LateUpdate()
    {
        thirdPerson.FollowTarget();

        isInteracting = animator.GetBool("isInteracting");
    }

    public void Movement()
    {
        if(isInteracting)
        {
            return;
        }
        if (controller.isGrounded)
        {
            if (moveDir.normalized.magnitude > 0.1f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }

            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Abs(h) + Mathf.Abs(v);

        Vector3 moveInput = new Vector3(h, 0, v).normalized;

        moveDir = (thirdPerson.PlanarRotation * moveInput);

        if (moveAmount > 0)
        {
            controller.Move(moveDir * Time.deltaTime * speed);
            targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        Jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;

        aud.PlayOneShot(steps[Random.Range(0, steps.Length)], stepVol);

        if (isSprinting)
            yield return new WaitForSeconds(0.3f);
        else if (animator.GetFloat("speed") > 0.9f)
            yield return new WaitForSeconds(0.4f);
        else
            yield return new WaitForSeconds(0.5f);

        isPlayingStep = false;
    }

    private void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpForce;
        }
    }

    private void UpdateAnimValues()
    {
        float currentSpeed = animator.GetFloat("speed");
        Mathf.Clamp01(currentSpeed);
        animator.SetFloat("speed", Mathf.Lerp(currentSpeed, moveDir.magnitude, Time.deltaTime * animTransSpeed));
        if (isSprinting)
        {
            animator.SetBool("sprint", true);
        }
        else
        {
            animator.SetBool("sprint", false);
        }
    }
}
