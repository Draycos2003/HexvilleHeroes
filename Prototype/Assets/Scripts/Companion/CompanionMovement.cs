using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CompanionMoevement : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]
    private playerController playerController;

    [SerializeField]
    private Player Player;

    [SerializeField]
    private Companion Companion;

    [Header("Idle Configs")]
    [SerializeField]
    [Range(0, 10f)]
    private float rotationSpeed = 2f;

    [Header("Follow Configs")]
    [SerializeField]
    private float followRadius = 2f;

    private Coroutine RoutateCo;
    //private Coroutine StateChangeCo;

    private bool inRange;

    private void Awake()
    {
        inRange = true;
        agent = GetComponent<NavMeshAgent>();
        Player.OnStateChange += HandleStateChange;
    }

    private void Update()
    {
        if (!inRange)
        {
            Vector3 playerPos = (playerController.transform.position - transform.position); // normalized to get movement direction
            float playerDist = Vector3.Distance(playerPos, transform.position);


            if (playerDist > followRadius)
            {
               Vector3.MoveTowards(transform.position,playerPos, followRadius);
            }
        }
    }

    private void HandleStateChange(PlayerState oldState, PlayerState newState) {

        //if (StateChangeCo != null) {

        //    StopCoroutine(StateChangeCo);

        //}

        switch (newState) {

            case PlayerState.idle:
                HandleIdlePlayer();
                break;
            case PlayerState.moving:
                HandleMovingPlayer();
                break;
        }

    }

    private void HandleMovingPlayer() {

        Companion.ChangeState(CompanionState.follow);
        //if (movementCo != null) {

        //    StopCoroutine(movementCo);
        //}

        //if (agent != null) {

        //    agent.enabled = true;
        //    agent.Warp(transform.position); 
        //}
        //movementCo = StartCoroutine(FollowPlayer());
        inRange = false;

        Vector3 playerPos = (playerController.transform.position - transform.position).normalized; // normalized to get movement direction
        float playerDist = Vector3.Distance(transform.position, playerPos);


        if(playerDist > followRadius)
        {
            Vector3.MoveTowards(transform.position, playerPos, agent.stoppingDistance);
        }


    }

    private IEnumerator RotateCompanion() {

        WaitForFixedUpdate Wait = new WaitForFixedUpdate();
        while (true) {

            transform.RotateAround(playerController.transform.position, Vector3.up, rotationSpeed);
            yield return Wait;
        }
    }

    private IEnumerator FollowPlayer() {

        yield return null; // Wait for player to move

        Vector3 offSet = followRadius * new Vector3(
            Mathf.Cos(2 * Mathf.PI * Random.value), 
            0,
            Mathf.Sin(2 * Mathf.PI * Random.value)).normalized;
      
        Debug.Log(agent.SetDestination(playerController.transform.position + offSet));
       
        yield return null; // wait for agent's desination
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        Companion.ChangeState(CompanionState.idle);
    }

    private IEnumerator HandleIdlePlayer()    {

        switch (Companion.State)        {

            case CompanionState.follow:
                yield return null;
                yield return null;
                //StopCoroutine(RoutateCo);
                yield return new WaitUntil(() => Companion.State == CompanionState.idle);
                goto case CompanionState.idle;
            
            case CompanionState.idle:
                //if ( != null)   {

                //    StopCoroutine(movementCo);

                //}
                agent.enabled = false;
                inRange = true;
                RoutateCo = StartCoroutine(RotateCompanion());
                break;
        }
    }

}
