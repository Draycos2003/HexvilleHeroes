using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int speed;
    [SerializeField] CharacterController contorller;
    Vector3 moveDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        contorller.Move(moveDir * speed * Time.deltaTime);
    }
}
