using UnityEngine;

public class mouseFollower : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Camera mainCam;

    [SerializeField] inventoryItemUI item;

    public void Start()
    {
        canvas = transform.root.GetComponent<Canvas>();
        mainCam = Camera.main;
        item = GetComponentInChildren<inventoryItemUI>();
        gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite,  int quanity)
    {
        item.SetData(sprite, quanity);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void Toggle(bool val)
    {
        if (val) Update();
        Debug.Log($"Item toggled {val}");
        gameObject.SetActive(val);
    }
}