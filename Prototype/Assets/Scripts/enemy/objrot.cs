using UnityEngine;

public class objrot : MonoBehaviour
{
    Transform tran;


    Vector3 to = new Vector3(90, 0, 0);

     = Vector3.Lerp(transform.rotation.eulerAngles, to, Time.deltaTime);
}
