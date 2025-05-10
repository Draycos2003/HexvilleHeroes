using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorOrig;

    Vector3 PlayerDir;

    float shootTimer;

    bool playerInRange;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        if (playerInRange)
        {
            PlayerDir = gamemanager.instance.Player.transform.position - transform.position;

            agent.SetDestination(gamemanager.instance.Player.transform.position);

            if (shootTimer >= shootRate)
            {
                shoot();
            }

            if (agent.remainingDistance < agent.stoppingDistance)
            {
                faceTarget();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void TakeDamage(int Amount)
    {
        HP -= Amount;

        if (HP <= 0) {
            Destroy(gameObject);
            gamemanager.instance.updateGameGoal(-1);

        } else {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
    }

    void faceTarget() // can add arg here for player to face
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(PlayerDir.x, transform.position.y, PlayerDir.z));

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);

    }

    void shoot()
    {
        shootTimer = 0;

        Instantiate(bullet, shootPos.position, transform.rotation);

    }
}
