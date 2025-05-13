using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    // World 
    [SerializeField] int gravity;

    // Player
    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    int jumpCount;

    [SerializeField] int HP;
    public int HPOrig => HP;

    [SerializeField] int speed;
    [SerializeField] int sprintMod;

    [SerializeField] int jumpMax;
    [SerializeField] int jumpForce;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = gamemanager.instance.Player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Attacking();
        Movement();
        Sprint();
    }

    void Movement()
    {
        
        if (controller.isGrounded && jumpCount != 0) 
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        //transform.position += moveDir * speed * Time.deltaTime;
        controller.Move(moveDir * speed * Time.deltaTime);

        Jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
   
    }

    void Sprint()
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

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;

            playerVel.y = jumpForce;
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        // check for death

        if (HP <= 0)
        {
            gamemanager.instance.youLose(); 
        }
    }

    void Attacking()
    {
        if (Input.GetButtonDown("Vertical"))
        {
            animator.SetBool("isWalking", true);
        }
        else if (Input.GetButtonUp("Vertical"))
        {
            animator.SetBool("isWalking", false);
        }
    }
}
