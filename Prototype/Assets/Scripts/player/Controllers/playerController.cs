using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using FinalController;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    #region Singleton
    public static playerController instance;
    #endregion

    #region Inspector Fields

    [Header("Controllers")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private Damage damage;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] walkAudio;
    [SerializeField] private AudioClip[] jumpAudio;
    [SerializeField] private AudioClip[] attackAudio;
    [SerializeField] private AudioClip[] damageAudio;
    [SerializeField] private float walkVolume;
    [SerializeField] private float jumpVolume;
    [SerializeField] private float attackVolume;
    [SerializeField] private float damageVolume;

    [Header("World Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AudioSource switchWeaponSoundSource;

    [Header("Player Stats")]
    public int HP;
    public int Shield;
    [SerializeField] private int walkSpeed = 3;
    [SerializeField] private int runSpeed = 5;
    [SerializeField] private int sprintSpeed = 10;
    [SerializeField] private int sprintMod;
    [SerializeField] private int jumpMax;
    [SerializeField] private int jumpHeight;
    [SerializeField] private int gold;

    [Header("Player UI")]
    public GameObject playerMenu;
    public GameObject inventoryCanvas;

    [Header("Weapon")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private Transform shootPos;
    [SerializeField] private int damageWithoutAWeapon;
    public int damageAmount;
    [HideInInspector] public float shootRate;
    [HideInInspector] public int shootDist;
    [HideInInspector] public agentWeapon weaponAgent;

    [Header("Inventory")]
    [SerializeField] private GameObject itemModel;
    [SerializeField] private GameObject equipSlot;

    [Header("Animation")]
    [SerializeField] private float animTransSpeed;
    [SerializeField] private float rotationTolerance = 90f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float rotationTime = 0.25f;

    #endregion

    #region Public Properties

    public int Gold => gold;
    public int HPOrig => HP;
    public int MAXHPOrig => maxHP;
    public int ShieldOrig => Shield;
    public int MAXShieldOrig => maxShield;
    public InventoryItem item { get; private set; }
    public float rotationMismatch { get; private set; } = 0f;
    public bool isRotating { get; private set; } = false;

    #endregion

    #region Private Fields

    private int maxHP;
    private int maxShield;
    private int currentSceneIndex;
    private int originalSceneIndex;

    private List<ItemParameter> parameters;

    private Vector3 moveDir;
    private float speedCur;
    private Vector3 velocity;
    private int jumpCount;
    private float shootTimer;
    private bool isPlayerStep;
    private bool isGrounded;
    private bool topOfJump;

    Quaternion targetRotation;
    private ThirdPersonCamController thirdPerson;
    playerInput playerLocomotionInput;
    playerState playerState;

    private Animator animator;
    private float rotationTimer;
    private bool isRotatingClockwise;
    private int allTimeDamageBuffAmount;
    private float stepOffsetOrig;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        controller = GetComponent<CharacterController>();

        
    }

    private void Start()
    {
        thirdPerson = FindFirstObjectByType<ThirdPersonCamController>();
        playerLocomotionInput = GetComponent<playerInput>();
        playerState = GetComponent<playerState>();
        animator = GetComponent<Animator>();
        damage = GetComponentInChildren<Damage>();
        maxHP = HP;
        maxShield = Shield;
        weaponAgent = GetComponent<agentWeapon>();
        damageAmount = damageWithoutAWeapon;
        stepOffsetOrig = controller.stepOffset;
    }

    private void Update()
    {
        shootTimer += Time.deltaTime;
        HandleAllMovement();
        UpdateInventoryItem();
        SetAnimParams();
    }

    private void LateUpdate()
    {
        thirdPerson.FollowTarget();
    }

    #endregion

    #region Scene Management

    public void SetSceneIndex(int newSceneIndex)
    {
        originalSceneIndex = currentSceneIndex;
        currentSceneIndex = newSceneIndex;
        Debug.Log($"[Player] Scene changed: from {originalSceneIndex} to {currentSceneIndex}");
    }

    public int GetCurrentSceneIndex() => currentSceneIndex;
    public int GetOriginalSceneIndex() => originalSceneIndex;

    #endregion

    #region Movement

    private void HandleAllMovement()
    {
        VerticalMovement();
        LateralMovement();
        UpdateMovmentState();
    }

    private void UpdateMovmentState()
    {
        bool canRun = CanRun();
        bool isMovementInput = playerLocomotionInput.movementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = playerLocomotionInput.sprintToggledOn && isMovingLaterally;
        bool isWalking = (isMovingLaterally && !canRun) || playerLocomotionInput.walkToggledOn;

        isGrounded = controller.isGrounded;

        PlayerMovementState lateralState =  isWalking ? PlayerMovementState.Walking :
                                isSprinting ? PlayerMovementState.Sprinting :
                               isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

        playerState.SetPlayerMovementState(lateralState);

        float peak = jumpHeight / 2;

        if(velocity.y <= peak) { topOfJump = true; } else {  topOfJump = false; }

        // control airborn state
        if (!isGrounded && velocity.y > 0 && !topOfJump)
        {
            playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
            controller.stepOffset = 0;
        }
        else if(!isGrounded && (velocity.y > 0 || velocity.y < 0))
        {
            playerState.SetPlayerMovementState(PlayerMovementState.Falling);
            controller.stepOffset = 0;
        }
        controller.stepOffset = stepOffsetOrig;
    }

    private void LateralMovement()
    {
        // State dependent move speed
        bool isSprinting = playerState.playerMovementStateCur == PlayerMovementState.Sprinting;
        bool isWalking = playerState.playerMovementStateCur == PlayerMovementState.Walking;
        bool isGrounded = controller.isGrounded;

        float speed = !isGrounded ? speedCur :
                      isWalking ? walkSpeed :
                      isSprinting ? sprintSpeed : runSpeed;

        speedCur = speed;

        if (isGrounded)
        {
            if (moveDir.normalized.magnitude > 0.1f && !isPlayerStep)
            {
                StartCoroutine(PlayStepSound());
            }
            jumpCount = 0;
            velocity = Vector3.zero;
        }

        float h = playerLocomotionInput.movementInput.x;
        float v = playerLocomotionInput.movementInput.y;

        float moveAmount = Mathf.Abs(h) + Mathf.Abs(v);

        Vector3 moveInput = new Vector3(h, 0, v).normalized;

        moveDir = (thirdPerson.PlanarRotation * moveInput);

        UpdatePlayerRotation();

        if (moveAmount > 0)
        {
            controller.Move(moveDir * Time.deltaTime * speed);
        }
    }

    private void VerticalMovement()
    {
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        if (IsGrounded() && velocity.y <= 0)
        {
            velocity.y = -3f; // small downward force to stay grounded
        }

        // Jump input
        if (playerLocomotionInput.jumpPressed && jumpCount < jumpMax)
        {
            jumpCount++;
            PlayJumpSound();
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (IsGroundedWhileAirborn())
        {
            velocity = HandleSteepWalls(velocity);
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdatePlayerRotation()
    {
        // if rotation mismatch is not within tolerance. or rotation timer is active, rotate
        // also rotate if moving
        bool isIdling = playerState.playerMovementStateCur == PlayerMovementState.Idling;
        isRotating = rotationTimer > 0;

        if (!isIdling)
        {
            RotatePlayerToTarget();
        }
        else if( Mathf.Abs(rotationMismatch) > rotationTolerance || isRotating)
        {
            UpdateIdleRotation(rotationTolerance);
        }

        Vector3 camForwardProjectedXZ = new Vector3(thirdPerson.transform.forward.x, 0f, thirdPerson.transform.forward.z).normalized;
        Vector3 crossProduct = Vector3.Cross(transform.forward, camForwardProjectedXZ);
        float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
        rotationMismatch = sign * Vector3.Angle(transform.forward, camForwardProjectedXZ);
    }

    private void UpdateIdleRotation(float rotationTolerence)
    {
        // initiate new rotation direction
        if (Mathf.Abs(rotationMismatch) > rotationTolerance)
        {
            rotationTimer = rotationTime;
            isRotatingClockwise = rotationMismatch > rotationTolerance;
        }
        rotationTimer -= Time.deltaTime;

        if((isRotatingClockwise && rotationMismatch > 0f) || (!isRotatingClockwise && rotationMismatch < 0f))
        {
            RotatePlayerToTarget();
        }
    }

    private void RotatePlayerToTarget()
    {
        targetRotation = Quaternion.LookRotation(new Vector3(thirdPerson.transform.forward.x, 0f, thirdPerson.transform.forward.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private Vector3 HandleSteepWalls(Vector3 vel)
    {
        Vector3 normal = controllerUtils.GetNormalWithSphereCast(controller, groundMask);
        float angle = Vector3.Angle(normal, Vector3.up);
        bool validAngle = angle >= controller.slopeLimit;

        if (validAngle && velocity.y < 0f)
        {
            vel = Vector3.ProjectOnPlane(vel, normal);
        }
        return vel;
    }

    #endregion

    #region State Checks

    private bool IsMovingLaterally()
    {
        return playerLocomotionInput.movementInput != Vector2.zero;
    }

    private bool CanRun()
    {
       // this means player is moving diagonally at 45 degrees or forward, if so, player can run
       return playerLocomotionInput.movementInput.y >= Mathf.Abs(playerLocomotionInput.movementInput.x);
    }

    private bool IsGrounded()
    {
        bool grounded = playerState.isGroundedState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborn();

        return grounded;
    }

    private bool IsGroundedWhileGrounded()
    {
        bool grounded = Physics.CheckSphere(groundCheck.position, controller.radius, groundMask, QueryTriggerInteraction.Ignore);

        return grounded;
    }

    private bool IsGroundedWhileAirborn()
    {
        Vector3 normal = controllerUtils.GetNormalWithSphereCast(controller, groundMask);
        float angle = Vector3.Angle(normal, Vector3.up);
        bool validAngle = angle >= controller.slopeLimit;

        return controller.isGrounded && validAngle;
    }

    #endregion

    #region Damage & Health

    public void TakeDamage(int amount)
    {
        if (Shield > 0)
        {
            Shield -= amount;
            StartCoroutine(FlashShieldDamageScreen());
            soundFXmanager.instance.PlaySoundFXClip(damageAudio[Random.Range(0, damageAudio.Length)], transform, damageVolume);
        }
        else
        {
            HP -= amount;
            StartCoroutine(FlashDamageScreen());
            soundFXmanager.instance.PlaySoundFXClip(damageAudio[Random.Range(0, damageAudio.Length)], transform, damageVolume);
        }

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    private IEnumerator FlashDamageScreen()
    {
        gamemanager.instance.playerDMGScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDMGScreen.SetActive(false);
    }

    private IEnumerator FlashShieldDamageScreen()
    {
        gamemanager.instance.playerShieldDMGScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerShieldDMGScreen.SetActive(false);
    }

    public void gainHealth(int amount)
    {
        if (HP < maxHP) HP += amount;
        if (HP > maxHP) HP = maxHP;
    }

    public void gainShield(int amount)
    {
        if (Shield < maxShield) Shield += amount;
        if (Shield > maxShield) Shield = maxShield;
    }

    public void gainDamage(int amount)
    {
        if (damage != null)
        {
            allTimeDamageBuffAmount += amount;
            damageWithoutAWeapon += amount;
        }
    }

    public void gainSpeed(int amount)
    {
        runSpeed += amount;
    }

    #endregion

    #region Inventory & Equipment

    private void UpdateInventoryItem()
    {
        item = GetComponent<inventoryController>().inventoryData.inventoryItems.Last();
        Equip();
    }

    private void Equip()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && !equipSlot.activeSelf)
        {
            equipSlot.SetActive(true);
        }

        if (!item.isEmpty)
        {
            ChangeEquippedItem();
        }
        else
        {
            NoItemEquipped();
        }
    }

    private void ChangeEquippedItem()
    {
        itemModel.GetComponent<MeshFilter>().sharedMesh = item.item.model.GetComponent<MeshFilter>().sharedMesh;
        itemModel.GetComponent<MeshRenderer>().sharedMaterial = item.item.model.GetComponent<MeshRenderer>().sharedMaterial;
        GetItemStats();
    }

    private void NoItemEquipped()
    {
        itemModel.GetComponent<MeshFilter>().sharedMesh = null;
        itemModel.GetComponent<MeshRenderer>().sharedMaterial = null;
        damageAmount = damageWithoutAWeapon;
    }

    private void GetItemStats()
    {
        damageAmount = (int)weaponAgent.FindParameterValue("Damage") + allTimeDamageBuffAmount;
        if (item.item.IType == ItemSO.ItemType.ranged)
        {
            shootRate = weaponAgent.FindParameterValue("Shoot Rate");
            shootDist = (int)weaponAgent.FindParameterValue("Range");
        }
    }

    #endregion

    #region Weapon & Shooting

    public void weaponColOn()
    {
        weapon.GetComponent<Collider>().enabled = true;
    }

    public void weaponColOff()
    {
        weapon.GetComponent<Collider>().enabled = false;
    }

    public void shoot()
    {
        Instantiate(item.item.projectile, shootPos.position, Camera.main.transform.rotation);
    }

    #endregion

    #region Animation

    private void SetAnimParams()
    {
        float currentSpeed = animator.GetFloat("speed");
        Mathf.Clamp01(currentSpeed);
        animator.SetFloat("speed", Mathf.Lerp(currentSpeed, moveDir.magnitude, Time.deltaTime * animTransSpeed));
    }

    #endregion

    #region Currency

    public bool RemoveGold(int cost)
    {
        if (gold < cost)
        {
            Debug.Log("Not enough gold!");
            return false;
        }

        gold -= cost;
        Debug.Log($"Purchased for {cost} gold. Remaining: {gold}");
        return true;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Sold item for {amount} gold. Total now: {gold}");
    }

    #endregion

    #region Audio

    private IEnumerator PlayStepSound()
    {
        isPlayerStep = true;
        soundFXmanager.instance.PlaySoundFXClip(walkAudio[Random.Range(0, walkAudio.Length)], transform, walkVolume);

        if (playerState.playerMovementStateCur == PlayerMovementState.Sprinting)
            yield return new WaitForSeconds(0.3f);
        else if (playerState.playerMovementStateCur == PlayerMovementState.Running)
            yield return new WaitForSeconds(0.4f);
        else
            yield return new WaitForSeconds(0.5f);

        isPlayerStep = false;
    }

    private void PlayJumpSound()
    {
        soundFXmanager.instance.PlaySoundFXClip(jumpAudio[Random.Range(0, jumpAudio.Length)], transform, jumpVolume);
    }

    #endregion
}
