using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

public class playerController : MonoBehaviour, IDamage
{
    // Player
    [Header("Controllers")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Damage damage;

    [Header("World")] // World 
    [SerializeField] int gravity;
    [SerializeField] AudioSource switchWeaponSoundSource;

    [Header("Player")] // Player
    public int HP;
    public int Shield;
    public GameObject playerMenu;
    public GameObject inventoryCanvas;

    public int HPOrig => HP;
    private int maxHP;
    public int ShieldOrig => Shield;
    private int maxShield;

    public int speed;
    public int speedOG;
    [SerializeField] int sprintMod;

    [SerializeField] int jumpMax;
    [SerializeField] int jumpForce;

    [Header("Currency")] // Currency
    [SerializeField] private int gold;
    public int Gold => gold;

    private int currentSceneIndex;
    private int originalSceneIndex;
    private List<ItemParameter> parameters;
    private Camera cam;

    [Header("Weapon")] // Weapon
    [SerializeField] GameObject weapon;
    [SerializeField] Transform shootPos;
    [HideInInspector] public int damageAmount;
    [HideInInspector] public float shootRate;
    [HideInInspector] public int shootDist;
    [SerializeField] int damageWithoutAWeapon;

    [Header("Inventory")]
    [SerializeField] GameObject itemModel;
    public InventoryItem item;
    agentWeapon weaponAgent;

    Vector3 moveDir;
    Vector3 playerVel;
    bool isSprinting;
    int jumpCount;
    float shootTimer;

    private Animator animator;
    [SerializeField] float animTransSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        speedOG = speed;
        damage = gameObject.GetComponentInChildren<Damage>();
        maxHP = HP;
        maxShield = Shield;
        weaponAgent = gameObject.GetComponent<agentWeapon>();
        damageAmount = damageWithoutAWeapon;
        shootRate = 0;
        shootDist = 0;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        Movement();
        Sprint();
        item = GetComponent<inventoryController>().inventoryData.inventoryItems.Last();
        equip();
        setAnimParams();
    }

    public void SetSceneIndex(int newSceneIndex)
    {
        originalSceneIndex = currentSceneIndex;
        currentSceneIndex = newSceneIndex;

        Debug.Log($"[Player] Scene changed: from {originalSceneIndex} to {currentSceneIndex}");
    }

    public int GetCurrentSceneIndex()
    {
        return currentSceneIndex;
    }

    public int GetOriginalSceneIndex()
    {
        return originalSceneIndex;
    }

    void Movement()
    {
        if (controller.isGrounded && jumpCount != 0)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
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

            animator.SetBool("isJumping", true);
        }

        if (Input.GetButtonUp("Jump"))
        {
            animator.SetBool("isJumping", false);
        }
    }

    public void TakeDamage(int amount)
    {
        if (Shield > 0)
        {
            Shield -= amount;
            StartCoroutine(flashShieldDamageScreen());
        }
        else
        {
            HP -= amount;
            StartCoroutine(flashDamageScreen());
        }

        // check for death
        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerDMGScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDMGScreen.SetActive(false);
    }

    IEnumerator flashShieldDamageScreen()
    {
        gamemanager.instance.playerShieldDMGScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerShieldDMGScreen.SetActive(false);
    }

    public void gainHealth(int amount)
    {
        // check if player is damaged
        if (HP < maxHP)
        {
            HP += amount;
        }

        // make sure health doesn't exceed max
        if (HP > maxHP)
        {
            HP = maxHP;
        }
    }

    public void gainShield(int amount)
    {
        // check if player needs shield
        if (Shield < maxShield)
        {
            Shield += amount;
        }

        // make sure shield doesn't exceed max
        if (Shield > maxShield)
        {
            Shield = maxShield;
        }
    }

    public void gainDamage(int amount)
    {
        if (damage != null)
        {
            Debug.Log("HAHAHA");
            damage.damageAmount += amount;
        }
    }

    public void gainSpeed(int amount)
    {
        speed += amount;
    }

    private void equip()
    {
        if (!item.isEmpty)
        {
            changeEquippedItem();
        }
        else
        {
            noItemEquipped();
        }
    }

    private void changeEquippedItem()
    {
        itemModel.GetComponent<MeshFilter>().sharedMesh = item.item.model.GetComponent<MeshFilter>().sharedMesh;
        itemModel.GetComponent<MeshRenderer>().sharedMaterial = item.item.model.GetComponent<MeshRenderer>().sharedMaterial;
        getItemStats();
    }

    private void noItemEquipped()
    {
        itemModel.GetComponent<MeshFilter>().sharedMesh = null;
        itemModel.GetComponent<MeshRenderer>().sharedMaterial = null;
        damageAmount = damageWithoutAWeapon;
        shootRate = 0;
        shootDist = 0;
    }

    private void getItemStats()
    {
        damageAmount = (int)weaponAgent.FindParameterValue("Damage");
        if (item.item.IType == ItemSO.ItemType.ranged)
        {
            shootRate = weaponAgent.FindParameterValue("Shoot Rate");
            shootDist = (int)weaponAgent.FindParameterValue("Range");
        }
        //weaponAgent.ModifyParameters();
    }
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

    private void setAnimParams()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(item.isEmpty)
            {
                animator.SetTrigger("attack");
                return;
            }

            if(shootTimer >= shootRate)
            {
                if (item.item.IType == ItemSO.ItemType.ranged)
                {
                    animator.SetTrigger("shoot");
                }
                shootTimer = 0;
            }
           

            if(item.item.IType == ItemSO.ItemType.melee)
            {
                animator.SetTrigger("attack");
            }
        }

        float animSpeedCur = animator.GetFloat("speed");
        animator.SetFloat("speed", Mathf.Lerp(animSpeedCur, moveDir.magnitude, Time.deltaTime * animTransSpeed));
    }

    public bool BuyItem(int cost)
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

    public void SellItem(int amount)
    {
        gold += amount;
        Debug.Log($"Sold item for {amount} gold. Total now: {gold}");
    }
}