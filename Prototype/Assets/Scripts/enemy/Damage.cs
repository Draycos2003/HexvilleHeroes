using UnityEngine;

public class Damage : MonoBehaviour
{
    enum damageType
    {
        moving, homing, DOT, stationary
    }

    [SerializeField] damageType type;

    [SerializeField] Rigidbody body;

    [SerializeField] int damageAmount;
    [SerializeField] int damageRate;
    [SerializeField] int speed;
    [SerializeField] int destoryTime;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
