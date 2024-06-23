using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Game-----------------")]
    [SerializeField] int alive, sumNumberCharacter, gold;
    [SerializeField] float countDownSpawnEnemy;
    [SerializeField] bool isGameOver, isGameStart, isGameWin, isRevivePlayer;
    [SerializeField] DataPlayer dataPlayer;
    [SerializeField] FollowCamera camManager;
    [SerializeField] PlayerManager player;
    [SerializeField] PlayerAnimation animPlayer;
    [SerializeField] EnemyManager[] enemys;
    [SerializeField] GameObject[] characters;
    [SerializeField] Transform[] posCharacters;
    [SerializeField] CapsuleCollider[] colliCharacters;
    [SerializeField] Material[] colorBodys, colorPants;
    [Header("HomeUI--------------")]
    [SerializeField] GameObject homeUI, goShopUI;
    [SerializeField] RawImage onSoundHome, offSoundHome, onVibrationHome, offVibrationHome;
    [SerializeField] Text numGoldHome;
    [Header("GamePlayUI----------")]
    [SerializeField] int speedInfinityUI;
    [SerializeField] GameObject gamePlayUI, settingUI, settingBtnUI, numAliveUI, infinityUI;
    [SerializeField] RawImage onSound, offSound, onVibration, offVibration, iconHand;
    [SerializeField] Text numAlive;
    Coroutine runInfinityWait = null;
    [Header("GameWinUI-----------")]
    [SerializeField] GameObject gameWinUI;
    [SerializeField] Text namePlayer, numGoldGameWin;
    [Header("GameOverUI----------")]
    [SerializeField] float timeLoadGameOver;
    [SerializeField] int speedRotateTimeGameOver;
    [SerializeField] GameObject gameOverUI, reviveNowUI, rankInfoUI, touchToContinueUI;
    [SerializeField] RawImage loadCircleGameOver;
    [SerializeField] Text timeLoadGameOverText, numRank, nameEnemyKillPlayer, numGoldGameOver;
    [Header("WeaponShopUI--------")]
    [SerializeField] int indexTabChoose, indexSkinWeaponChoose, indexSkinWeaponCur, indexColorPickerWeapon;
    [SerializeField] GameObject camWeaponUI, weaponUI, colorPickerUI;
    [SerializeField] List<int> weaponCustoms = new List<int>();
    [SerializeField] Color[] colorPickers = new Color[12];
    [SerializeField] Image[] colorPickersImg;
    [SerializeField] GameObject[] tabWeaponUI, indexColorUI;
    [SerializeField] MeshRenderer[] skinWeapon, skinWeponDisplay;
    [SerializeField] RectTransform[] posUseBtn;
    [Header("SkinShopUI----------")]
    [SerializeField] int indexTabSkinCur;
    [SerializeField] Text attributeSet;
    [SerializeField] GameObject camSkinUI, skinUI;
    [SerializeField] Image[] tabIconSkinUI, isChoosePants, isChooseHairs, isChooseShield, isChooseSet;
    [SerializeField] GameObject[] tabSkinUI, hairs, shields, adsSkinBuyBtn;
    [SerializeField] RectTransform[] posUseSkinBtn;

    void Awake()
    {
        Instance = this;
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

        if (PlayerPrefs.GetInt(Constant.SOUND) != 0)
        {
            onSoundHome.enabled = true;
            offSoundHome.enabled = false;
        }
        else
        {
            onSoundHome.enabled = false;
            offSoundHome.enabled = true;
        }

        if (PlayerPrefs.GetInt(Constant.VIBRATION) != 0)
        {
            onVibrationHome.enabled = true;
            offVibrationHome.enabled = false;
        }
        else
        {
            onVibrationHome.enabled = false;
            offVibrationHome.enabled = true;
        }

        this.gold = dataPlayer.LoadGame().Gold;
        indexSkinWeaponCur = dataPlayer.LoadGame().IndexSkinWeaponCur;
        numGoldHome.text = this.gold.ToString();
    }
    void Start()
    {
        InitListWeaponCusDefault();
        weaponCustoms = dataPlayer.LoadColorWeeaponCustom().WeaponColorCustoms;
        for(int i = 0; i <= player.IndexWeaponOpen; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (j < skinWeapon[i * 5].materials.Length)
                {
                    skinWeapon[i * 5].materials[j].color = colorPickers[weaponCustoms[i * 3 + j]];
                    colorPickersImg[i * 3 + j].color = colorPickers[weaponCustoms[i * 3 + j]];
                }
                else
                {
                    continue;
                }
            }
        }
        player.ChangeColorWeaponCustom();

        for (int i = 0; i < Constant.NUMCHARACTER1TURN - 1; i++)
            enemys[i].SetNameEnemy("Enemy" + (i + 1));
    }
    void Update()
    {
        SpawnEnemy();
        if (player.IsMove)
        {
            infinityUI.SetActive(false);
            StopCoroutine(runInfinityWait);
        }
        if (isGameOver)
        {
            infinityUI.SetActive(false);
            StopCoroutine(runInfinityWait);
            GameOverSquence();
        }
        if (alive == 1 && player.enabled == true)
        {
            GameWinSquence();
        }
    }

    void InitListWeaponCusDefault()
    {
        for (int i = 0; i <= player.IndexWeaponOpen; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                weaponCustoms.Add(0);
            }
        }
    }

    IEnumerator RunInfinity()   
    {
        float elapsed = 0f;
        float duration = 30f;

        while (elapsed < duration)
        {
            iconHand.transform.localPosition = new Vector3(Mathf.Cos(elapsed), Mathf.Sin(elapsed * 2) / 2) * speedInfinityUI - new Vector3(0, 25);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void SpawnEnemy()
    {
        for (int i = 1; i < Constant.NUMCHARACTER1TURN; i++)
        {
            if (characters[i].activeSelf == false && sumNumberCharacter < 50 - Constant.NUMCHARACTER1TURN)
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
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //UI Home
    public void GoToGamePlayUI()
    {
        isGameStart = true;
        camManager.MoveCameraToPlayGame();

        homeUI.SetActive(false);
        gamePlayUI.SetActive(true);
        if (PlayerPrefs.GetInt(Constant.SOUND) != 0)
        {
            onSound.enabled = true;
            offSound.enabled = false;
        }
        else
        {
            onSound.enabled = false;
            offSound.enabled = true;
        }
        if (PlayerPrefs.GetInt(Constant.VIBRATION) != 0)
        {
            onVibration.enabled = true;
            offVibration.enabled = false;
        }
        else
        {
            onVibration.enabled = false;
            offVibration.enabled = true;
        }
        infinityUI.SetActive(true);
        runInfinityWait = StartCoroutine(RunInfinity());

        player.SetTextNamePlayer();
        player.Info.SetActive(true);
        foreach (EnemyManager enemy in enemys)
        {
            enemy.GetInfo().SetActive(true);
        }

        player.SpawnWeaponPlayer();
        player.CircleRangeAtk.SetActive(true);
        animPlayer.LoadWeaponData();
    }
    public void GoToZombieCity(int sceneID)
    {
        player.InstantiateWeapon();
        player.Weapon.tag = Constant.WEAPONDATA;
        DontDestroyOnLoad(player.Weapon);
        SceneManager.LoadScene(sceneID);
    }
    public void GoWeaponUI()
    {
        goShopUI.SetActive(false);
        camWeaponUI.SetActive(true);
        SwitchTabWerponUI(player.IndexWeaponCur);
        ChooseSkinWeapon(indexSkinWeaponCur);
        weaponUI.SetActive(true);
        player.gameObject.SetActive(false);
    }
    public void GoSkinUI()
    {
        goShopUI.SetActive(false);
        camSkinUI.SetActive(true);
        skinUI.SetActive(true);
        player.StateAnim = StateAnimation.Dance;
        SwitchTabSkinUI(player.GetIndexSkinCur());
        player.AutoSetChangeTab(player.GetIndexSkinCur());
        camManager.MoveCameraToSkinUI(1);
    }
    public void OnOffSoundHome()
    {
        onSoundHome.enabled = !onSoundHome.enabled;
        offSoundHome.enabled = !offSoundHome.enabled;
        if (onSoundHome.enabled)
        {
            PlayerPrefs.SetInt(Constant.SOUND, 1);
        }
        else
        {
            PlayerPrefs.SetInt(Constant.SOUND, 0);
        }
    }
    public void OnOffVibrationHome()
    {
        onVibrationHome.enabled = !onVibrationHome.enabled;
        offVibrationHome.enabled = !offVibrationHome.enabled;
        if (onVibrationHome.enabled)
        {
            PlayerPrefs.SetInt(Constant.VIBRATION, 1);
        }
        else
        {
            PlayerPrefs.SetInt(Constant.VIBRATION, 0);
        }
    }

    //UI Game Play
    public void NumberEnemyAlive()
    {
        alive--;
        numAlive.text = "Alive: " + alive;
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
        if (onSound.enabled)
        {
            PlayerPrefs.SetInt(Constant.SOUND, 1);
        }
        else
        {
            PlayerPrefs.SetInt(Constant.SOUND, 0);
        }
    }
    public void OnOffVibration()
    {  
        onVibration.enabled = !onVibration.enabled;
        offVibration.enabled = !offVibration.enabled;
        if (onVibration.enabled)
        {
            PlayerPrefs.SetInt(Constant.VIBRATION, 1);
        }
        else
        {
            PlayerPrefs.SetInt(Constant.VIBRATION, 0);
        }
    }
    public void PlayGoHomeUI()
    {
        isGameStart = false;
        dataPlayer.SaveGame();
        RestartGame();
    }

    //UI Game Win
    void GameWinSquence()
    {
        isGameWin = true;
        numGoldGameWin.text = (player.Level * 2).ToString();
        namePlayer.text = player.NamePlayer;
        gameWinUI.SetActive(true);
    }

    //UI Game Over
    void GameOverSquence()
    {
        gameOverUI.SetActive(true);
        if (timeLoadGameOver > 0 && !isRevivePlayer)
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
    public void CloseReviveNow()
    {
        reviveNowUI.SetActive(false);
        touchToContinueUI.SetActive(true);
        rankInfoUI.SetActive(true);
    }
    public void RevivePlayer(int index)
    {
        if (index == 0)
        {
            if (gold >= 150)
            {
                gold -= 150;
            }
            else
            {
                return;
            }
        }
        player.RevivePlayer();
        isGameOver = false;
        isRevivePlayer = true;
        reviveNowUI.SetActive(false);
        gameOverUI.SetActive(false);
    }
    public void SetNumRank(int numRank)
    {
        this.numRank.text = "# " + numRank.ToString();
    }
    public void SetNameEnemyKillPlayer(string nameEnemyKillPlayer)
    {
        this.nameEnemyKillPlayer.text = nameEnemyKillPlayer;
    }
    public void SetNumGold(int numGold)
    {
        numGoldGameOver.text = numGold.ToString();
        gold += numGold;
    }

    //UI Weapon Shop
    public void WeaponUIGoHomeUI()
    {
        goShopUI.SetActive(true);
        camWeaponUI.SetActive(false);
        player.ReadyOpenWeaponUI();
        weaponUI.SetActive(false);
        player.gameObject.SetActive(true);
    }
    public void SwitchTabWerponUI(int indexTab)
    {
        indexTabChoose = indexTab;
        tabWeaponUI[Mathf.Clamp(indexTab - 1, 0, tabWeaponUI.Length - 1)].SetActive(false);
        tabWeaponUI[Mathf.Clamp(indexTab + 1, 0, tabWeaponUI.Length - 1)].SetActive(false);
        tabWeaponUI[indexTab].SetActive(true);
        
        if (indexTabChoose != player.IndexWeaponCur)
        {
            ChooseSkinWeaponCustom(false);
            ChooseSkinWeapon(2);
        }
        else
        {
            if (1 < indexSkinWeaponChoose && indexSkinWeaponChoose < 5)
            {
                ChooseSkinWeaponCustom(false);
                ChooseSkinWeapon(indexSkinWeaponCur);
            }
            else if (indexSkinWeaponChoose == 0)
            {
                ChooseSkinWeaponCustom(true);
                ChooseSkinWeapon(0);
            }
        }
    }
    public void ChooseSkinWeapon(int index)
    {
        indexSkinWeaponChoose = index;

        if (player.IndexWeaponCur == indexTabChoose && indexSkinWeaponChoose == indexSkinWeaponCur)
        {
            player.GetTextWeaponUses()[indexTabChoose].text = Constant.EQUIPPED;
        }
        else
        {
            player.GetTextWeaponUses()[indexTabChoose].text = Constant.SELECT;
        }

        if (index == 0)
        {
            ChooseSkinWeaponCustom(true);
            indexColorPickerWeapon = 0;
        }
        else
        {
            ChooseSkinWeaponCustom(false);
        }
        skinWeponDisplay[indexTabChoose].materials = skinWeapon[index + indexTabChoose * 5].materials;
    }
    void ChooseSkinWeaponCustom(bool b)
    {
        colorPickerUI.SetActive(b);
        indexColorUI[indexTabChoose].SetActive(b);
        if (b)
        {
            posUseBtn[indexTabChoose].anchoredPosition = Vector2.down * 875;
        }
        else
        {
            posUseBtn[indexTabChoose].anchoredPosition = Vector2.down * 615;
        }
    }
    public void ColorPicker(int index)
    {
        weaponCustoms[indexTabChoose * 3 + indexColorPickerWeapon] = index;
        colorPickersImg[indexTabChoose * 3 + indexColorPickerWeapon].color = colorPickers[index];
        skinWeapon[indexTabChoose * 5].materials[indexColorPickerWeapon].color = colorPickers[index];
    }
    public void SetIndexColorPickerWeapon(int index)
    {
        indexColorPickerWeapon = index;
    }

    //UI Skin Shop
    public void SkinUIGoHomeUI()
    {
        goShopUI.SetActive(true);
        camSkinUI.SetActive(false);
        skinUI.SetActive(false);
        player.StateAnim = StateAnimation.Idle;
        player.LoadSkinCur();
        camManager.MoveCameraToSkinUI(-1);
    }
    public void SwitchTabSkinUI(int indexTab)
    {
        indexTabSkinCur = indexTab;
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
    public void SwitchChooseSkin(int indexChoose)
    {
        if (indexChoose < 0)
        {
            indexChoose = 0;
        }
        if (indexTabSkinCur == (int)SkinOrder.Hair)
        {
            foreach (Image hair in isChooseHairs)
                hair.enabled = false;
            foreach (int i in player.HairsBought)
                if (i == indexChoose)
                {
                    ChooseSkinBought(true);
                    break;
                }
                else
                {
                    ChooseSkinBought(false);
                }
            isChooseHairs[indexChoose].enabled = true;
        }
        else if (indexTabSkinCur == (int)SkinOrder.Pant)
        {
            foreach (Image pant in isChoosePants)
                pant.enabled = false;
            foreach (int i in player.PantsBought)
                if (i == indexChoose)
                {
                    ChooseSkinBought(true);
                    break;
                }
                else
                {
                    ChooseSkinBought(false);
                }

            isChoosePants[indexChoose].enabled = true;
        }
        else if (indexTabSkinCur == (int)SkinOrder.Shield)
        {
            foreach (Image shield in isChooseShield)
                shield.enabled = false;
            foreach (int i in player.ShieldsBought)
                if (i == indexChoose)
                {
                    ChooseSkinBought(true);
                    break;
                }
                else
                {
                    ChooseSkinBought(false);
                }

            isChooseShield[indexChoose].enabled = true;
        }
        else if (indexTabSkinCur == (int)SkinOrder.Set)
        {
            foreach (Image set in isChooseSet)
                set.enabled = false;

            isChooseSet[indexChoose].enabled = true;
            attributeSet.text = dataPlayer.GetSetData(indexChoose).attribute;
        }
    }
     public void ChooseSkinBought(bool b)
    {
        if (b)
        {
            posUseSkinBtn[indexTabSkinCur].anchoredPosition = new Vector2(0, -520);
            adsSkinBuyBtn[indexTabSkinCur].SetActive(false);
        }
        else
        {
            posUseSkinBtn[indexTabSkinCur].anchoredPosition = new Vector2(-300, -520);
            adsSkinBuyBtn[indexTabSkinCur].SetActive(true);
        }
    }

    //get, set
    public GameObject[] Characters
    {
        get {
            return characters;
        }
    }
    public Transform[] PosEnemy
    {
        get
        {
            return posCharacters;
        }
    }
    public CapsuleCollider[] ColliCharacter
    {
        get
        {
            return colliCharacters;
        }
    }
    public EnemyManager[] Enemys
    {
        get
        {
            return enemys;
        }
    }
    public Material[] ColorBodys
    {
        get
        {
            return colorBodys;
        }
    }
    public Material[] ColorPants
    {
        get
        {
            return colorPants;
        }
    }
    public GameObject[] Hairs
    {
        get
        {
            return hairs;
        }
    }
    public GameObject[] Shields
    {
        get
        {
            return shields;
        }
    }
    public int Alive
    {
        get
        {
            return alive;
        }
    }
    public bool IsGameOver
    {
        get
        {
            return isGameOver;
        }
        set
        {
            isGameOver = value;
        }      
    }
    public bool IsStartGame
    {
        get
        {
            return isGameStart;
        }
    }
    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
        }
    }
    public PlayerManager Player
    {
        get
        {
            return player;
        }
    }
    public DataPlayer DataPlayer
    {
        get
        {
            return dataPlayer;
        }

    }
    public GameObject[] TabWeaponUI
    {
        get
        {
            return tabWeaponUI;
        }
    }
    public int NumGoldHome
    {
        set
        {
            numGoldHome.text = value.ToString();
        }
    }
    public bool IsGameWin
    {
        get
        {
            return isGameWin;
        }
    }
    public bool IsRevivePlayer
    {
        get
        {
            return isRevivePlayer;
        }
    }
    public int IndexSkinWeaponCur
    {
        get
        {
            return indexSkinWeaponCur;
        }
        set
        {
            indexSkinWeaponCur = value;
        }
    }
    public int IndexSkinWeaponChoose
    {
        get
        {
            return indexSkinWeaponChoose;
        }
    }
    public MeshRenderer[] SkinWeapon
    {
        get
        {
            return skinWeapon;
        }
    }
    public List<int> WeaponCustoms
    {
        get
        {
            return weaponCustoms;
        }
    }
}
