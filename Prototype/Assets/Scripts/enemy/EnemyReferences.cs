using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyReferences : MonoBehaviour
{
    public NavMeshAgent navMesh;
    public Animator animate;
    enemyAI enemy;

    public float pathUpdateDely = 0.2f;

    private void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        animate = GetComponent<Animator>();
    }


    void Update()
    {
        
    }
}
