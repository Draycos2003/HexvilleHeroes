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
    private ObjectPool(PoolableObject prefab, int Size)
    {
        this.prefab = prefab;
        this.size = Size;
        avalilableObjects = new List<PoolableObject>(Size);
    }

    public static ObjectPool CreateInstance(PoolableObject prefab, int Size)
    {
        ObjectPool pool = null;

        if (ObjectPools.ContainsKey(prefab))
        {
            pool = ObjectPools[prefab]; // Object pool is now not empty
        }
        else
        {
            pool = new ObjectPool(prefab, Size);

            pool.parent = new GameObject(prefab + "Pool");
            pool.CreateObjects();

            ObjectPools.Add(prefab, pool);
        }
        return pool;
    }

    private void CreateObjects()
    {
        for (int i = 0; i <= size; i++)
        {
            CreateObject();
        }
    }

    private void CreateObject()
    {
        PoolableObject poolableObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent.transform);
        poolableObject.parent = this;
        poolableObject.gameObject.SetActive(false);
    }

    public PoolableObject GetObject (Vector3 Position, Quaternion Rotation)
    {
        if(avalilableObjects.Count == 0)
        {
            CreateObject();
        }

        PoolableObject instance = avalilableObjects[0]; // instance is the first object
       
        avalilableObjects.RemoveAt(0); // remove the first object from the list

        instance.transform.position = Position; // reset the position
        instance.transform.rotation = Rotation; // and rotation

        instance.gameObject.SetActive(true); // Make the game object active(useable)

        return instance; // return the object to where ever GetObject is being called.
    }

    public PoolableObject GetObject()
    {
        return GetObject(Vector3.zero, Quaternion.identity);
    }

    public void ReturnObjectToPool(PoolableObject Object)
    {
        avalilableObjects.Add(Object);
    }

}
