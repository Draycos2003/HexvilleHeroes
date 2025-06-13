using UnityEngine;
using System.Collections.Generic;


public class ObjectPool
{
    private GameObject parent; 
    private PoolableObject prefab;
    private int size; // size of pool
    private List<PoolableObject> avalilableObjects;
    private static Dictionary<PoolableObject, ObjectPool> ObjectPools = new Dictionary<PoolableObject, ObjectPool>();
    
    //Constructor
    private ObjectPool(PoolableObject Prefab, int Size)
    {
        this.prefab = Prefab;
        this.size = Size;
        avalilableObjects = new List<PoolableObject>(Size);
    }

    public static ObjectPool CreateInstance(PoolableObject prefab, int Size)
    {
        ObjectPool pool = null;

        if (ObjectPools.ContainsKey(prefab))
        {
            pool = ObjectPools[prefab]; // pool now holds the prefab
        }
        else
        {
            pool = new ObjectPool(prefab, Size); // Create a new pool holding the set amount of prefabs (EX: the pool will hold 10 projectile prefabs)

            pool.parent = new GameObject(prefab + "Pool"); // The pools parent becomes the prefab(EX:ProjectPool); 
            pool.CreateObjects(); // Create the wanted amount of objects

            ObjectPools.Add(prefab, pool); // Add the prefab and the pool holding all the prefabs to the Disctionary of pools
        }
        return pool; // return the pool holding everything to where ever its being called
    }

    private void CreateObjects()
    {
        for (int i = 0; i < size; i++)
        {
            CreateObject();
        }
    }

    private void CreateObject()
    {
        PoolableObject poolableObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent.transform);
        poolableObject.parent = this;
        poolableObject.gameObject.SetActive(false); // on creation the game object is set to false (unuseable);
    }

    public PoolableObject GetObject (Vector3 Position, Quaternion Rotation)
    {
       
        if(avalilableObjects.Count == 0)
        {
            CreateObject();
        }


        PoolableObject instance = avalilableObjects[0]; // instance is the first object
       
        avalilableObjects.RemoveAt(0); // remove the first object from the list of avalilable objects

        instance.transform.position = Position; // reset the position
        instance.transform.rotation = Rotation; // and rotation

        instance.gameObject.SetActive(true); // Make the game object active(useable)

        return instance; // return the object to where ever GetObject is being called.
    }


    public PoolableObject GetObject() // this verison of GetObject is always called first to then GetObject with parameters is called to return the instance of the object.
    {
        return GetObject(Vector3.zero, Quaternion.identity);
    }

    public void ReturnObjectToPool(PoolableObject Object)
    {
        avalilableObjects.Add(Object); // Once the object is turned false it is added back to the avalilableObjects list. How it works can be found in the PoolableObjects script
    }

}
