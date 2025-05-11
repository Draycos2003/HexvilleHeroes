using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;

public class EnemyShooter : MonoBehaviour
{
    [Header("General")]

    public Transform shootPoint; 
    public Transform gunPoint;
    public LayerMask layer;

    [Header("Weapon")]

    public TrailRenderer trail;
    public GameObject projectile;
    public float Distance;
    private EnemyReferences reference;


    public 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        Vector3 direction = projectile.transform.forward;
        Physics.Raycast(shootPoint.position, direction, out RaycastHit hit, Distance, layer);
        Debug.DrawLine(shootPoint.position, shootPoint.position + direction * 10f, Color.red, 10f);

        // look into object pooling

        trail = Instantiate(trail, gunPoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, hit));
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0f;

        Vector3 startPostition = trail.transform.position;

        while(time < 1f)
        {
            trail.transform.position = Vector3.Lerp(startPostition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }

        trail.transform.position = hit.point;
        Destroy(trail.gameObject, trail.time);
    }
}
