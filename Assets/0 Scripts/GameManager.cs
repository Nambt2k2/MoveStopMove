using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Game")]
    [SerializeField] FollowCamera camManager;
    [SerializeField] GameObject[] characters, wepons, hairs, shields, sets;
    [SerializeField] Transform[] posCharacters;
    [SerializeField] CapsuleCollider[] colliCharacters;
    [SerializeField] EnemyManager[] enemys;
    [SerializeField] PlayerManager player;
    [SerializeField] Material[] colorBodys, colorPants;
    [SerializeField] int alive, sumNumberCharacter;
    [SerializeField] float countDownSpawnEnemy;
    [SerializeField] bool isGameOver, isGameStart;
    [Header("UI")]
    [SerializeField] GameObject homeUI, gamePlayUI, camUI3D, skinUI, goShopUI, gameOverUI;
    [SerializeField] GameObject reviveNowUI, rankInfoUI, touchToContinueUI, settingUI, numAliveUI, settingBtnUI;
    [SerializeField] GameObject[] tabSkinUI;
    [SerializeField] Image[] tabIconSkinUI;
    [SerializeField] Text numAlive, timeLoadGameOverText, numRank, nameEnemyKillPlayer, numGold;
    [SerializeField] RawImage loadCircleGameOver, onSound, offSound, onVibration, offVibration;
    [SerializeField] int speedRotate;
    [SerializeField] float timeLoadGameOver;

    void Awake()
    {
        posCharacters = new Transform[Constant.NUMCHARACTER1TURN];
        colliCharacters = new CapsuleCollider[Constant.NUMCHARACTER1TURN];
        enemys = new EnemyManager[Constant.NUMCHARACTER1TURN - 1];

        for (int i = 0; i < Constant.NUMCHARACTER1TURN; i++)
        {
            posCharacters[i] = characters[i].transform;
            colliCharacters[i] = characters[i].GetComponent<CapsuleCollider>();
        }

        for (int i = 0; i < Constant.NUMCHARACTER1TURN - 1; i++)
        {
            enemys[i] = characters[i + 1].GetComponent<EnemyManager>();
        }

        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < Constant.NUMCHARACTER1TURN - 1; i++)
            enemys[i].SetNameEnemy("Enemy" + (i + 1));
    }

    void Update()
    {
        SpawnEnemy();

        if (isGameOver)
        {
            GameOverSquence();
        }
    }

    void SpawnEnemy()
    {
        for (int i = 1; i < Constant.NUMCHARACTER1TURN; i++)
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
    }

    void GameOverSquence()
    {
        gameOverUI.SetActive(true);
        if (timeLoadGameOver > 0)
        {
            timeLoadGameOver -= Time.deltaTime;
            loadCircleGameOver.transform.Rotate(Vector3.forward * speedRotate * Time.deltaTime);
            timeLoadGameOverText.text = ((int)timeLoadGameOver).ToString();
            settingUI.SetActive(false);
            numAliveUI.SetActive(false);
            settingBtnUI.SetActive(false);
        }
        else
        {
            if (isGameStart)
            {
                reviveNowUI.SetActive(false);
                touchToContinueUI.SetActive(true);
                rankInfoUI.SetActive(true);
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
        reviveNowUI.SetActive(false);
        touchToContinueUI.SetActive(true);
        rankInfoUI.SetActive(true);
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
    public void DisplaySettingUI(bool b)
    {
        settingUI.SetActive(b);
        numAliveUI.SetActive(!b);
        settingBtnUI.SetActive(!b);
    }
    public void OnOffSound()
    {
        onSound.enabled = !onSound.enabled;
        offSound.enabled = !offSound.enabled;
    }
    public void OnOffVibration()
    {
        onVibration.enabled = !onVibration.enabled;
        offVibration.enabled = !offVibration.enabled;
    }
    public void GoToGamePlayUI()
    {
        isGameStart = true;
        homeUI.SetActive(false);
        gamePlayUI.SetActive(true);
        player.GetCircleRangeAtk().SetActive(true);
        player.GetInfo().SetActive(true);
        foreach (EnemyManager enemy in enemys)
        {
            enemy.GetInfo().SetActive(true);
        }
        camManager.MoveCameraToPlayGame();
    }
    public void PlayGoHomeUI()
    {
        isGameStart = false;
        RestartGame();
    }
    public void GoSkinUI()
    {
        goShopUI.SetActive(false);
        camUI3D.SetActive(true);
    }
    public void SkinUIGoHomeUI()
    {
        goShopUI.SetActive(true);
        camUI3D.SetActive(false);

    }
    public void SwitchTabSkinUI(int index)
    {
        foreach (GameObject tab in tabSkinUI)
        {
            tab.SetActive(false);
        }
        tabSkinUI[index].SetActive(true);
        foreach (Image tab in tabIconSkinUI)
        {
            tab.enabled = true;
        }
        tabIconSkinUI[index].enabled = false;
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
    public Material[] GetBody()
    {
        return colorBodys;
    }
    public Material[] GetPant()
    {
        return colorPants;
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
    public bool GetIsStartGame()
    {
        return isGameStart;
    }
}
