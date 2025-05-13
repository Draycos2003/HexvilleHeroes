using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class attackArea : MonoBehaviour
{
    // initialize list of damagable objects witin the attack area 
    public List<IDamage> Damagables { get; } = new();

    // add objects in the attack area to the list
    public void OnTriggerEnter(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();

        if(dmg != null)
        {
            Damagables.Add(dmg);
        }
    }

    // remove objects from the list when they leave the attack area
    public void OnTriggerExit(Collider other)
    {
        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && Damagables.Contains(dmg))
        {
            Damagables.Remove(dmg);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
