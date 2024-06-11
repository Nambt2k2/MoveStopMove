using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject[] characters;
    [SerializeField] Transform[] posCharacters;
    [SerializeField] CapsuleCollider[] colliCharacters;
    [SerializeField] EnemyManager[] enemys;
    [SerializeField] int Alive;
    [SerializeField] Text aliveText;


    void Awake()
    {
        posCharacters = new Transform[10];
        colliCharacters = new CapsuleCollider[10];
        enemys = new EnemyManager[9];

        for (int i = 0; i < 10; i++)
        {
            posCharacters[i] = characters[i].transform;
            colliCharacters[i] = characters[i].GetComponent<CapsuleCollider>();
        }
        for (int i = 0; i < 9; i++)
        {
            enemys[i] = characters[i + 1].GetComponent<EnemyManager>();
        }

        Instance = this;
    }

    void Update()
    {
    
        for (int i = 1; i < 10; i++)
        {
            if (characters[i].activeSelf == false)
                enemys[i - 1].Spwan();
        }
    }

    public void EnemyDie()
    {
        Alive--;
        aliveText.text = "Alive: " + Alive;
    }

    public void NewRound()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public GameObject[] GetCharacter()
    {
        return characters;
    }
    public Transform[] GetPosEnemy()
    {
        return posCharacters;
    }
    public CapsuleCollider[] GetColliCharacter()
    {
        return colliCharacters;
    }
    public EnemyManager[] GetEnemy()
    {
        return enemys;
    }
}
