using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    GameObject tmp;
    List<List<GameObject>> pooledObjects;
    [SerializeField] GameObject[] objectInPool;
    [SerializeField] List<GameObject> zombieBegin;

    void Awake()
    {
        Instance = this;
        pooledObjects = new List<List<GameObject>>();
        pooledObjects.Add(zombieBegin);
        pooledObjects.Add(new List<GameObject>());

    }

    public GameObject GetPooledObject(ObjectInPool index)
    {
        foreach (GameObject tmp in pooledObjects[(int) index])
        {
            if (!tmp.activeInHierarchy)
            {
                return tmp;
            }
        }

        tmp = Instantiate(objectInPool[(int) index]);
        tmp.SetActive(false);
        pooledObjects[(int)index].Add(tmp);
        return tmp;
    }

    public List<List<GameObject>> PooledObjects
    {
        get
        {
            return pooledObjects;
        }
    }

    public enum ObjectInPool { Zombie, Weapon }
}
