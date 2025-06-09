using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

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
    [SerializeField] public int speed;
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

    [Header("Inventory")]
    [SerializeField] private GameObject itemModel;
    [SerializeField] private GameObject equipSlot;

    [Header("Animation")]
    [SerializeField] private float animTransSpeed;

    [Header("States")]
    [SerializeField]
    private Player player;

    #endregion

    #region Public Properties

    public int Gold => gold;
    public int HPOrig => HP;
    public int MAXHPOrig => maxHP;
    public int ShieldOrig => Shield;
    public int MAXShieldOrig => maxShield;
    public int speedOG { get; private set; }
    public InventoryItem item { get; private set; }

    #endregion

    #region Private Fields

    private int maxHP;
    private int maxShield;
    private int currentSceneIndex;
    private int originalSceneIndex;

    private List<ItemParameter> parameters;
    private Camera cam;
    private agentWeapon weaponAgent;

    private Vector3 moveDir;
    private Vector3 playerVel;
    private bool isSprinting;
    private int jumpCount;
    private float shootTimer;
    private bool isPlayerStep;

    private Animator animator;
    private int allTimeDamageBuffAmount;

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
        cam = Camera.main;
        animator = GetComponent<Animator>();
        speedOG = speed;
        damage = GetComponentInChildren<Damage>();
        maxHP = HP;
        maxShield = Shield;
        weaponAgent = GetComponent<agentWeapon>();
        damageAmount = damageWithoutAWeapon;
    }

    private void Update()
    {
        shootTimer += Time.deltaTime;
        if (!Movement()) player.ChangeState(PlayerState.moving);
        Sprint();
        UpdateInventoryItem();
        SetAnimParams();
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

    private bool Movement()
    {
        if (controller.isGrounded && jumpCount != 0)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime); 
        player.ChangeState(PlayerState.moving);

        Jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        return true;
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
            speed = speedOG;
            isSprinting = false;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpForce;
            soundFXmanager.instance.PlaySoundFXClip(jumpAudio[Random.Range(0, jumpAudio.Length)], transform, jumpVolume);
            animator.SetBool("isJumping", true);
        }

        if (Input.GetButtonUp("Jump"))
        {
            animator.SetBool("isJumping", false);
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
        speed += amount;
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
        if (Input.GetMouseButtonDown(0))
        {
            if (item.isEmpty)
            {
                animator.SetTrigger("attack");
                return;
            }

            if (shootTimer >= shootRate && item.item.IType == ItemSO.ItemType.ranged)
            {
                animator.SetTrigger("shoot");
                shootTimer = 0;
            }
            else if (item.item.IType == ItemSO.ItemType.melee)
            {
                animator.SetTrigger("attack");
            }
        }

        float currentSpeed = animator.GetFloat("speed");
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

    public void WalkSound()
    {
        if (controller.isGrounded && moveDir.normalized.magnitude > 0.3f && !isPlayerStep)
        {
            StartCoroutine(PlayStep());
        }
    }

    private IEnumerator PlayStep()
    {
        isPlayerStep = true;
        soundFXmanager.instance.PlaySoundFXClip(walkAudio[Random.Range(0, walkAudio.Length)], transform, walkVolume);

        if (isSprinting)
            yield return new WaitForSeconds(0.3f);
        else
            yield return new WaitForSeconds(0.5f);

        isPlayerStep = false;
    }

    #endregion
}
