using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject[] characters;
    [SerializeField] Transform[] posCharacters;
    [SerializeField] CapsuleCollider[] colliCharacters;
    [SerializeField] EnemyManager[] enemys;
    [SerializeField] int alive, sumNumberCharacter;
    [SerializeField] float countDownSpawnEnemy;
    [SerializeField] bool isGameOver;

    [SerializeField] GameObject ReviveNowUI, RankInfoUI, TouchToContinue;
    [SerializeField] Text numAlive, timeLoadGameOverText, numRank, nameEnemyKillPlayer, numGold;
    [SerializeField] Canvas gameOverUI;
    [SerializeField] RawImage loadCircleGameOver;
    [SerializeField] int speedRotate;
    [SerializeField] float timeLoadGameOver;

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
            enemys[i].SetNameEnemy("enemy" + (i + 1));
        }

        Instance = this;
    }

    void Update()
    {
        for (int i = 1; i < 10; i++)
        {
            if (characters[i].activeSelf == false && sumNumberCharacter < 40)
            {
                countDownSpawnEnemy -= Time.deltaTime;
                if (countDownSpawnEnemy < 0)
                {
                    enemys[i - 1].Spwan();
                    sumNumberCharacter++;
                    countDownSpawnEnemy = 2;
                }
            }
        }

        if (isGameOver)
        {
            gameOverUI.enabled = true;
            if (timeLoadGameOver > 0)
            {
                timeLoadGameOver -= Time.deltaTime;
                loadCircleGameOver.transform.Rotate(Vector3.forward * speedRotate * Time.deltaTime);
                timeLoadGameOverText.text = ((int)timeLoadGameOver).ToString();
            }
            else
            {
                ReviveNowUI.SetActive(false);
                TouchToContinue.SetActive(true);
                RankInfoUI.SetActive(true);
            }
        }
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //UI
    public void NumberEnemyAlive()
    {
        alive--;
        numAlive.text = "Alive: " + alive;
    }
    public void CloseReviveNow()
    {
        ReviveNowUI.SetActive(false);
        TouchToContinue.SetActive(true);
        RankInfoUI.SetActive(true);
    }
    public void SetNumRank(int numRank) 
    {
        this.numRank.text += numRank.ToString();
    }
    public void SetNameEnemyKillPlayer(string nameEnemyKillPlayer) 
    {
        this.nameEnemyKillPlayer.text = nameEnemyKillPlayer;
    } 
    public void SetNumGold(int numGold)
    {
        this.numGold.text = numGold.ToString();
    }

    //get, set
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
    public int GetNumAlive()
    {
        return alive;
    }
    public void SetIsGameOver(bool b)
    {
        isGameOver = b;
    }
    public bool GetIsGameOver()
    {
        return isGameOver;
    }
}
