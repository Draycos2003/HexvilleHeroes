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

    [Header("Buffs")]
    [SerializeField] int buffStatAmount;
    
    private int currentSceneIndex;
    private int originalSceneIndex;

    [Header("Weapon")] // Weapon
    [SerializeField] int damageAmount;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    [Header("Inventory")]
    [SerializeField] GameObject itemModel;
    public InventoryItem item;

    Vector3 moveDir;
    Vector3 playerVel;
    bool isSprinting;
    int jumpCount;
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
        if(!item.isEmpty)
        {
            changeEquippedItem();
        }
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
        if(Shield > 0)
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

    public void gainHealth(int amount)
    {
        // check if player is damaged
        if (HP < maxHP)
        {
            HP += amount;
        }

        // make sure health doesn't exceed max
        if(HP > maxHP)
        {
            HP = maxHP;
        }
    }

    public void gainShield(int amount)
    {
        // check if player needs shield
        if(Shield < maxShield)
        {
            Shield += amount;
        }

        // make sure shield doesn't exceed max
        if(Shield > maxShield)
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

    public void changeEquippedItem()
    {
        item = GetComponent<inventoryController>().inventoryData.inventoryItems.Last();
        itemModel.GetComponent<MeshFilter>().sharedMesh = item.item.model.GetComponent<MeshFilter>().sharedMesh;
        itemModel.GetComponent<MeshRenderer>().sharedMaterial = item.item.model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void getItemStats(ItemSO weapon)
    {
       
    }
}
