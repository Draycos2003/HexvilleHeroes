
using UnityEngine;
using UnityEngine.AI;

public class CompanionMovement_1 : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    
    [SerializeField]
    private Transform companion;
    
    [SerializeField]
    [Range(5,10f)]
    private float companionSpeed;
    
    [SerializeField]
    [Range(0,10f)]
    private float companionRotateSpeed;
    
    [SerializeField]
    [Range(3,10)]
    private float allowedDistance;

    private float playerDist;
    private RaycastHit hit;
    
    private void Awake()
    {
    }


    void Update()
    {

        transform.LookAt(player.position);
        var playerPosition = player.position + (player.right * -4f + player.up); // off setting player position for companion

        if (Physics.Raycast(companion.transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            float playerDist = hit.distance;

            if (playerDist > allowedDistance)
            {
                companion.position = Vector3.MoveTowards(companion.position, playerPosition, companionSpeed * Time.deltaTime); // Companion movement
                //companion.rotation = (player.position - companion.position) * companionRotateSpeed * Time.deltaTime; // Companion rotation
            }
 

        }
    }
}
            