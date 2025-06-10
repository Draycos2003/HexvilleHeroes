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
    [SerializeField] private int gravity;
    [SerializeField] private AudioSource switchWeaponSoundSource;

    [Header("Player Stats")]
    public int HP;
    public int Shield;
    [SerializeField] private int runSpeed = 5;
    [SerializeField] private int sprintSpeed = 10;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int sprintMod;
    [SerializeField] private int jumpMax;
    [SerializeField] private int jumpForce;
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

    #endregion

    #region Public Properties

    public int Gold => gold;
    public int HPOrig => HP;
    public int MAXHPOrig => maxHP;
    public int ShieldOrig => Shield;
    public int MAXShieldOrig => maxShield;
    public InventoryItem item { get; private set; }

    #endregion

    #region Private Fields

    private int maxHP;
    private int maxShield;
    private int currentSceneIndex;
    private int originalSceneIndex;

    private List<ItemParameter> parameters;

    private Vector3 moveDir;
    private Vector3 playerVel;
    private int jumpCount;
    private float shootTimer;
    private bool isPlayerStep;

    Quaternion targetRotation;
    private ThirdPersonCamController thirdPerson;
    playerLocomotionInput playerLocomotionInput;
    playerState playerState;

    private Animator animator;
    private int allTimeDamageBuffAmount;
    private bool isInteracting;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        thirdPerson = FindFirstObjectByType<ThirdPersonCamController>();
        playerLocomotionInput = GetComponent<playerLocomotionInput>();
        playerState = GetComponent<playerState>();
        animator = GetComponent<Animator>();
        damage = GetComponentInChildren<Damage>();
        controller = GetComponent<CharacterController>();
        maxHP = HP;
        maxShield = Shield;
        weaponAgent = GetComponent<agentWeapon>();
        damageAmount = damageWithoutAWeapon;
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
        UpdateMovmentState();
        LateralMovement();
        FallingAndLanding();
    }

    private void UpdateMovmentState()
    {
        bool isMovementInput = playerLocomotionInput.movementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = playerLocomotionInput.sprintToggledOn && isMovingLaterally;

        PlayerMovementState lateralState =  isSprinting ? PlayerMovementState.Sprinting :
                               isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

        playerState.SetPlayerMovementState( lateralState );
    }

    private bool IsMovingLaterally()
    {
        return playerLocomotionInput.movementInput != Vector2.zero;
    }

    private void FallingAndLanding()
    {
        
    }

    private void LateralMovement()
    {
        if (isInteracting)
        {
            return;
        }
        if (controller.isGrounded)
        {
            if (this.moveDir.normalized.magnitude > 0.1f && !isPlayerStep)
            {
                StartCoroutine(PlayStep());
            }

            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        // State dependent move speed
        bool isSprinting = playerState.playerMovementStateCur == PlayerMovementState.Sprinting;
        float speed = isSprinting ? sprintSpeed : runSpeed;

        float h = playerLocomotionInput.movementInput.x;
        float v = playerLocomotionInput.movementInput.y;

        float moveAmount = Mathf.Abs(h) + Mathf.Abs(v);

        Vector3 moveInput = new Vector3(h, 0, v).normalized;

        moveDir = (thirdPerson.PlanarRotation * moveInput);

        if (moveAmount > 0)
        {
            controller.Move(speed * Time.deltaTime * moveDir);
            targetRotation = Quaternion.LookRotation(new Vector3(thirdPerson.transform.forward.x, 0f, thirdPerson.transform.forward.z));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpForce;
            soundFXmanager.instance.PlaySoundFXClip(jumpAudio[Random.Range(0, jumpAudio.Length)], transform, jumpVolume);

            controller.Move(playerVel * Time.deltaTime);
            playerVel.y -= gravity * Time.deltaTime;
        }
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

    private IEnumerator PlayStep()
    {
        isPlayerStep = true;
        soundFXmanager.instance.PlaySoundFXClip(walkAudio[Random.Range(0, walkAudio.Length)], transform, walkVolume);

        if (true)
            yield return new WaitForSeconds(0.3f);
        //else if (animator.GetFloat("speed") > 0.9f)
        //    yield return new WaitForSeconds(0.4f);
        //else
        //    yield return new WaitForSeconds(0.5f);

        isPlayerStep = false;
    }

    #endregion
}
