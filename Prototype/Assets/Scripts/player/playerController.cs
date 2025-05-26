using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    // Player
    [SerializeField] List<pickupItemStats> items = new List<pickupItemStats>();
    [SerializeField] GameObject itemModel;

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

    [Header("Buffs")]
    [SerializeField] int buffStatAmount;

    private int currentSceneIndex;
    private int originalSceneIndex;

    [Header("Weapon")] // Weapon
    [SerializeField] Transform equipPos;
    [SerializeField] int damageAmount;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    inventorymanager inventory;

    Vector3 moveDir;
    Vector3 playerVel;
    bool isSprinting;
    int jumpCount;
    public int itemListPos;
    float shootTimer;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speedOG = speed;
        inventory = inventorymanager.instance;
        damage = gameObject.GetComponentInChildren<Damage>();
        animator = GetComponent<Animator>();
        maxHP = HP;
        maxShield = Shield;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Sprint();
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

        animator.SetFloat("speed", moveDir.magnitude);


        Jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        selectItem();
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
        gamemanager.instance.playerDMGScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDMGScreen.SetActive(false);
    }

    public void buffPlayer()
    {

    }

    void gainHealth(int amount)
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

    void gainShield(int amount)
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

    void gainDamage(int amount)
    {
        if (damage != null)
        {
            Debug.Log("HAHAHA");
            damage.damageAmount += amount;
        }
    }

    void gainSpeed(int amount)
    {
        speed += amount;
    }

    void selectItem()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && itemListPos > 0)
        {
            itemListPos--;
            changeItem();
            inventory.ChangeSelectedSlot(itemListPos);
            switchWeaponSoundSource.Play();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && itemListPos < items.Count - 1)
        {
            if (itemListPos < inventory.hotBarSize - 1)
            {
                itemListPos++;
                changeItem();
                inventory.ChangeSelectedSlot(itemListPos);
                switchWeaponSoundSource.Play();
            }
        }
    }

    void changeItem()
    {
        damageAmount = items[itemListPos].damgageAmount;
        shootDist = items[itemListPos].shootRange;
        shootRate = items[itemListPos].shootRate;
        buffStatAmount = items[itemListPos].buffAmount;



        itemModel.GetComponent<MeshFilter>().sharedMesh = items[itemListPos].model.GetComponent<MeshFilter>().sharedMesh;
        itemModel.GetComponent<MeshRenderer>().sharedMaterial = items[itemListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void getItemStats(pickupItemStats weapon)
    {
        items.Add(weapon);
        itemListPos = items.Count - 1;

        if (itemListPos < inventory.hotBarSize)
        {
            inventory.ChangeSelectedSlot(itemListPos);
            changeItem();
        }
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