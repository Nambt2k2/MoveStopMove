using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Game")]
    [SerializeField] FollowCamera camManager;
    [SerializeField] GameObject[] characters, wepons, hairs, shields;
    [SerializeField] Transform[] posCharacters;
    [SerializeField] CapsuleCollider[] colliCharacters;
    [SerializeField] EnemyManager[] enemys;
    [SerializeField] PlayerManager player;
    [SerializeField] Material[] colorBodys, colorPants;

    [SerializeField] int alive, sumNumberCharacter;
    [SerializeField] float countDownSpawnEnemy;
    [SerializeField] bool isGameOver, isGameStart;
    [Header("UI")]
    [SerializeField] GameObject homeUI, gamePlayUI, camSkinUI, camWeaponUI, skinUI, goShopUI, gameOverUI;
    [SerializeField] GameObject reviveNowUI, rankInfoUI, touchToContinueUI, settingUI, numAliveUI, settingBtnUI;
    [SerializeField] GameObject[] tabSkinUI, tabWeaponUI;
    [SerializeField] Image[] tabIconSkinUI, isChoosePants, isChooseHairs, isChooseShield, isChooseSet;
    [SerializeField] Text numAlive, timeLoadGameOverText, numRank, nameEnemyKillPlayer, numGold;
    [SerializeField] RawImage loadCircleGameOver, onSound, offSound, onVibration, offVibration;
    [SerializeField] int speedRotateTimeGameOver, numberTabSkin;
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
            loadCircleGameOver.transform.Rotate(Vector3.forward * speedRotateTimeGameOver * Time.deltaTime);
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
        camSkinUI.SetActive(true);
        player.SetDance();
        player.SaveSkinCur();
        player.AutoSetChangeTab(0);
        camManager.MoveCameraToSkinUI(1);
    }
    public void SkinUIGoHomeUI()
    {
        goShopUI.SetActive(true);
        camSkinUI.SetActive(false);
        player.SetIdle();
        player.LoadSkinCur();
        camManager.MoveCameraToSkinUI(-1);
    }
    public void GoWeaponUI()
    {
        goShopUI.SetActive(false);
        camWeaponUI.SetActive(true);
        player.gameObject.SetActive(false);
    }
    public void WeaponUIGoHomeUI()
    {
        goShopUI.SetActive(true);
        camWeaponUI.SetActive(false);
        player.gameObject.SetActive(true);
    }
    public void SwitchTabSkinUI(int indexTab)
    {
        numberTabSkin = indexTab;
        SwitchChooseSkin(0);
        foreach (GameObject tab in tabSkinUI)
        {
            tab.SetActive(false);
        }

        tabSkinUI[indexTab].SetActive(true);
        player.AutoSetChangeTab(indexTab);

        foreach (Image tabIcon in tabIconSkinUI)
        {
            tabIcon.enabled = true;
        }
        tabIconSkinUI[indexTab].enabled = false;
    }

    public void SwitchTabWerponUI(int indexTab)
    {
        tabWeaponUI[Mathf.Clamp(indexTab - 1, 0, tabWeaponUI.Length - 1)].SetActive(false);
        tabWeaponUI[Mathf.Clamp(indexTab + 1, 0, tabWeaponUI.Length - 1)].SetActive(false);
        tabWeaponUI[indexTab].SetActive(true);
    }

    public void SwitchChooseSkin(int indexChoose)
    {
        if (numberTabSkin == 0)
        {
            foreach (Image hair in isChooseHairs)
            {
                hair.enabled = false;
            }
            isChooseHairs[indexChoose].enabled = true;
        }
        else if (numberTabSkin == 1)
        {
            foreach (Image pant in isChoosePants)
            {
                pant.enabled = false;
            }
            isChoosePants[indexChoose].enabled = true;
        }
        else if (numberTabSkin == 2)
        {
            foreach (Image shield in isChooseShield)
            {
                shield.enabled = false;
            }
            isChooseShield[indexChoose].enabled = true;
        }
        else
        {
            foreach (Image set in isChooseSet)
            {
                set.enabled = false;
            }
            isChooseSet[indexChoose].enabled = true;
        }
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
    public GameObject[] GetHair ()
    {
        return hairs;
    }
    public GameObject[] GetShield()
    {
        return shields;
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
