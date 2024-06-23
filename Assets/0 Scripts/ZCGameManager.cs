using UnityEngine;
using System.Collections;

public class ZCGameManager : MonoBehaviour
{
    public static ZCGameManager Instance;
    GameObject tmp;
    [SerializeField] ZCPlayerManager player;
    [SerializeField] int numZombieMax; 
    Vector3[] posZombieSpawn;
    [SerializeField] Color[] colors;

    void Awake()
    {
        Instance = this;
        posZombieSpawn = new Vector3[6];
        posZombieSpawn[0] = new Vector3(30.06f, 12.1f, 12.31f);
        posZombieSpawn[1] = new Vector3(30.06f, 12.1f, 32.17f);
        posZombieSpawn[2] = new Vector3(21.9f, 12.1f, 33.78f);
        posZombieSpawn[3] = new Vector3(15.18f, 12.1f, 24.93f);
        posZombieSpawn[4] = new Vector3(5.63f, 12.1f, 7.21f);
        posZombieSpawn[5] = new Vector3(26.54f, 12.1f, -3.93f);
    }

    void Start()
    {
        StartCoroutine(SpawnZombie());
    }

    void Update()
    {
        if (numZombieMax <= 0 || player.IsDie)
        {
            StopCoroutine(SpawnZombie());
        }
    }

    public Color[] Colors
    {
        get
        {
            return colors;
        }
    }

    IEnumerator SpawnZombie()
    {
        while (numZombieMax > 0)
        {
            Spawn1Zombie();
            Spawn1Zombie();
            Spawn1Zombie();
            Spawn1Zombie();
            Spawn1Zombie();
            numZombieMax -= 5;
            yield return new WaitForSeconds(5);
        }
    }

    void Spawn1Zombie()
    {
        tmp = ObjectPool.Instance.GetPooledObject(ObjectPool.ObjectInPool.Zombie);
        tmp.transform.position = posZombieSpawn[Random.Range(0, posZombieSpawn.Length)];
        tmp.SetActive(true);
    }
}
