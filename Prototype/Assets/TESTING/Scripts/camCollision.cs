using UnityEngine;

public class camCollision : MonoBehaviour
{

    public float minDistance, maxDistance;
    public float smooth;
    public Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float distance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude ;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredCamPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        if(Physics.Linecast(transform.parent.position, desiredCamPos, out hit))
        {
            distance = Mathf.Clamp(hit.distance * 0.85f, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}
