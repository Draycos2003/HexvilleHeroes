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
    private Transform playerPosition;

    [SerializeField]
    private Companion Companion;

    [Header("Idle Configs")]
    [SerializeField]
    [Range(0, 10f)]
    private float rotationSpeed = 2f;

    [Header("Follow Configs")]
    [SerializeField]
    private float followRadius = 2f;

    private Coroutine movementCo;
    private Coroutine StateChangeCo;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        Player.OnStateChange += HandleStateChange;
    }

    private void HandleStateChange(PlayerState oldState, PlayerState newState) {

        if (StateChangeCo != null) {

            StopCoroutine(StateChangeCo);

        }

        switch (newState) {

            case PlayerState.idle:
                StateChangeCo = StartCoroutine(HandleIdlePlayer());
                break;
            case PlayerState.moving:
                HandleMovingPlayer();
                break;
        }

    }

    private void HandleMovingPlayer() {

        Companion.ChangeState(CompanionState.follow);
        if (movementCo != null) {

            StopCoroutine(movementCo);
        }

        if (agent != null) {

            agent.enabled = true;
            agent.Warp(transform.position);
        }
        movementCo = StartCoroutine(FollowPlayer());
    }

    private IEnumerator RotateCompanion() {

        WaitForFixedUpdate Wait = new WaitForFixedUpdate();
        while (true) {

            transform.RotateAround(playerPosition.transform.position, Vector3.up, rotationSpeed);
            yield return Wait;
        }
    }

    private IEnumerator FollowPlayer() {

        yield return null; // Wait for player to move

        Vector3 offSet = followRadius * new Vector3(
            Mathf.Cos(2 * Mathf.PI * Random.value), 
            0,
            Mathf.Sin(2 * Mathf.PI * Random.value)).normalized;

        agent.SetDestination(playerPosition.transform.position);
        Debug.Log("Moving");


        yield return null; // wait for agent's desination
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        Companion.ChangeState(CompanionState.idle);
    }

    private IEnumerator HandleIdlePlayer()    {

        switch (Companion.State)        {

            case CompanionState.follow:
                yield return null;
                yield return null;
                yield return new WaitUntil(() => Companion.State == CompanionState.idle);
                goto case CompanionState.idle;
            
            case CompanionState.idle:
                if (movementCo != null)   {

                    StopCoroutine(movementCo);

                }
                agent.enabled = false;
                movementCo = StartCoroutine(RotateCompanion());
                break;
        }
    }

}
