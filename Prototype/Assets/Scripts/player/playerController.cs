using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    // Player


    [Header("Controllers")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("World")] // World 
    [SerializeField] int gravity;

    [Header("Player")] // Player
    [SerializeField] int HP;
 
    [SerializeField] int speed;
    [SerializeField] int sprintMod;

    [SerializeField] int jumpMax;
    [SerializeField] int jumpForce;

    [Header("Weapon")] // Weapon
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    public int HPOrig => HP;
    Vector3 moveDir;
    Vector3 playerVel;
    bool isSprinting;
    int jumpCount;

    float shootTimer;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = gamemanager.instance.Player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Vertical"))
        {
            animator.SetBool("isWalking", true);
        }
        else if (Input.GetButtonUp("Vertical"))
        {
            animator.SetBool("isWalking", false);
        } 

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        Movement();
        Sprint();

        

    }

    void Movement()
    {
        shootTimer += Time.deltaTime;

        
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

        if (Input.GetButtonDown("Fire1") && shootTimer > shootRate)
            Shoot();
   
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

    void Shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.transform.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
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
}
