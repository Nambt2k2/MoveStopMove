using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Move-----------")]
    [SerializeField] DynamicJoystick joytick;
    [Header("Weapon---------")]
    [SerializeField] Transform holdWeapon, posWeapon;
    [SerializeField] GameObject[] weaponPrefab;
    [SerializeField] GameObject weapon;
    [SerializeField] WeaponManager weaponController;
    [Header("Player---------")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] CapsuleCollider colli;
    [SerializeField] GameObject circleRangeAtk, infoPrefab, info;
    [SerializeField] Transform posInfo;
    [SerializeField] bool isMove, inRangeAtk, canAtk;
    [SerializeField] int moveSpeed, level;
    [SerializeField] float angle, angleAtkRotation, rangeAtk;
    [Header("PlayerUI--------")]
    [SerializeField] InputField inputNamePlayer;
    [SerializeField] Text namePlayer, textLevel;
    [Header("ShopWeaponUI----")]
    [SerializeField] int indexWeaponCur, indexWeaponOpen;
    [SerializeField] Text[] weaponBuys, weaponUses;
    [Header("PlayerSkin-----")]
    [SerializeField] SkinnedMeshRenderer body, pant;
    [SerializeField] Material pantCur, pantOrigin, bobyOrigin;
    [SerializeField] GameObject[] hairs, shields, sets;
    [SerializeField] Text hairBuy, hairUse;
    [SerializeField] int hairCur, shieldCur, setCur, indexSkinChoose;
    [SerializeField] bool isUseSet;
    [SerializeField] List<int> hairsBought;
    [Header("Enemys---------")]
    [SerializeField] float[] distances;
    [SerializeField] Vector3 posEnemy, directionEnemy;
    [Header("Camera---------")]
    [SerializeField] FollowCamera cam;
    [Header("Animation------")]
    [SerializeField] PlayerAnimation anim;
    [SerializeField] StateAnimation stateAnim = StateAnimation.Idle;

    void Awake()
    {
        distances = new float[Constant.NUMCHARACTER1TURN];
        info = Instantiate(infoPrefab);
        namePlayer = info.GetComponentsInChildren<Text>()[0];
        textLevel = info.GetComponentsInChildren<Text>()[1];
        info.SetActive(false);
        inputNamePlayer.text = GameManager.Instance.GetDataPlayer().LoadGame().GetNamePlayer();
        indexWeaponCur = GameManager.Instance.GetDataPlayer().LoadGame().GetIndexWeaponCur();
        indexWeaponOpen = GameManager.Instance.GetDataPlayer().LoadGame().GetIndexWeaponOpen();
        hairCur = GameManager.Instance.GetDataPlayer().LoadGame().GetIndexHairCur();
        SetHair(hairCur);
        hairsBought = GameManager.Instance.GetDataPlayer().LoadGame().GetHairBought();
        ReadyOpenWeaponUI();
        SpawnWeaponPlayer();
    }

    void Update()
    {
        if (GameManager.Instance.GetIsStartGame())
        {
            Move();
            CheckRangeAtk();
            if (!isMove && inRangeAtk && holdWeapon.gameObject.activeSelf && !weapon.activeSelf)
            {
                canAtk = true;
            }
            UpdateState();
        }
        anim.UpdateAnimation(stateAnim);
    }
    void FixedUpdate()
    {      
        if (weaponController.IsKillEnemy())
        {
            rangeAtk *= Constant.ZOOMLEVELUP;
            cam.ShrinkCamera();
            posInfo.localPosition *= Constant.SHRINKCAM;
            weaponController.ResertIsKillEnemy();
            level++;
            textLevel.text = level.ToString();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constant.WEAPON) && other.gameObject != weapon)
        {
            for (int i = 0; i < Constant.NUMCHARACTER1TURN - 1; i++)
            {
                GameManager.Instance.GetEnemy()[i].IsPlayerChooseAtk(false);
            }
            enabled = false;
            rigid.velocity = Vector3.zero;
            colli.enabled = false;
            circleRangeAtk.SetActive(false);
            anim.UpdateAnimation(StateAnimation.Dead);
            GameManager.Instance.SetIsGameOver(true);
            GameManager.Instance.SetNumRank(GameManager.Instance.GetNumAlive());
            GameManager.Instance.SetNameEnemyKillPlayer(
                other.gameObject.GetComponent<WeaponManager>().getCharacter().GetComponent<EnemyManager>().GetNameEnemy());
            GameManager.Instance.SetNumGold(level * 100);
        }
    }

    void Move()
    {
        Vector3 movement = rigid.velocity;
        movement.x = joytick.Horizontal;
        movement.z = joytick.Vertical;

        rigid.velocity = movement.normalized * moveSpeed;
        info.transform.position = posInfo.position;

        if (rigid.velocity.sqrMagnitude > 0)
        {
            angle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * angle;
            isMove = true;
        }
        else
        {
            isMove = false;
        }
    }

    //atk
    void CheckRangeAtk()
    {
        for (int i = 0; i < Constant.NUMCHARACTER1TURN; i++)
        {
            if (GameManager.Instance.GetCharacter()[i].activeSelf && GameManager.Instance.GetColliCharacter()[i].enabled)
            {
                distances[i] = (GameManager.Instance.GetPosEnemy()[i].position - transform.position).sqrMagnitude;
            }
            else
            {
                distances[i] = Constant.DISTANCEWHENDIE;
            }
        }

        float min = distances[0];

        for (int i = 1; i < Constant.NUMCHARACTER1TURN; i++)
        {
            if (distances[i] != 0)
            {
                min = distances[i];
                break;
            }
        }
        
        for (int i = 0; i < Constant.NUMCHARACTER1TURN; i++)
        {
            if (min > distances[i] && distances[i] != 0)
            {
                min = distances[i];
            }
        }

        if (min > rangeAtk * rangeAtk)
        {
            inRangeAtk = false;
            canAtk = false;
        }

        for (int i = 1; i < Constant.NUMCHARACTER1TURN; i++)
        {
            GameManager.Instance.GetEnemy()[i - 1].IsPlayerChooseAtk(false);
            if (min == distances[i] && min < rangeAtk * rangeAtk)
            {
                inRangeAtk = true;
                posEnemy = GameManager.Instance.GetPosEnemy()[i].position;
                GameManager.Instance.GetEnemy()[i - 1].IsPlayerChooseAtk(true);
            }
        }
    }
    public void AtkRotation()
    {
        if (!weapon.activeSelf)
        {
            directionEnemy = posEnemy - transform.position;
            angle = Mathf.Atan2(directionEnemy.x, directionEnemy.z) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * angle;
            weapon.transform.position = posWeapon.position;
            weapon.SetActive(true);
        }
        weapon.transform.Rotate(Vector3.forward * angleAtkRotation * Time.deltaTime);
        weapon.transform.position += directionEnemy.normalized * Time.deltaTime * rangeAtk * Constant.DISTANCEWEAPONALIVE;
    }
    public void SpawnWeaponPlayer()
    {
        weapon = Instantiate(weaponPrefab[indexWeaponCur]);
        weapon.SetActive(false);
        weaponController = weapon.GetComponent<WeaponManager>();
        weaponController.SetCharacter(transform);
    }

    //animation
    void UpdateState()
    {
        if (rigid.velocity.x != 0 || rigid.velocity.z != 0)
        {
            stateAnim = StateAnimation.Run;
        }
        else
        {
            stateAnim = StateAnimation.Idle;
            if (canAtk)
                stateAnim = StateAnimation.Attack;
        } 
    }

    //event animation
    public void SetFalseCanAtk()
    {
        canAtk = false;
    }
    public void SetFalseDiplayWeapon()
    {
        holdWeapon.gameObject.SetActive(false);
    }
    public void SetTrueDiplayWeapon()
    {
        holdWeapon.gameObject.SetActive(true);
    }

    //set, get
    public GameObject GetCircleRangeAtk()
    {
        return circleRangeAtk;
    }
    public GameObject GetInfo()
    {
        return info;
    }
    public void SetDance()
    {
        stateAnim = StateAnimation.Dance;
    }
    public void SetIdle()
    {
        stateAnim = StateAnimation.Idle;
    }
    public int GetIndexWeaponOpen()
    {
        return indexWeaponOpen;
    }
    public int GetIndexWeaponCur()
    {
        return indexWeaponCur;
    }
    public string GetNamePlayer()
    {
        return inputNamePlayer.text;
    }
    public Text GetHairBuyText() 
    {
        return hairBuy;    
    } 
    public Text GetHairUseText()
    {
        return hairUse;
    }
    public int GetHairCur()
    {
        return hairCur;
    }
    public List<int> GetHairsBought()
    {
        return hairsBought;
    }
    public int GetIndexSkinChoose()
    {
        return indexSkinChoose;
    }

    //UI
    public void BuyWeapon(int index)
    {
        int cost = 0;
        cost = GameManager.Instance.GetDataPlayer().GetWeaponData(index + 1).cost;

        if (indexWeaponOpen == index && cost <= GameManager.Instance.GetGold())
        {
            GameManager.Instance.SetGold(-cost);
            GameManager.Instance.SetNumGoldText(GameManager.Instance.GetGold());
            weaponBuys[indexWeaponOpen].gameObject.SetActive(false);
            indexWeaponOpen++;
            weaponUses[indexWeaponOpen].gameObject.SetActive(true);
        }
    }
    public void UseWeapon(int index)
    {
        if (indexWeaponCur != index && index <= indexWeaponOpen)
        {
            indexWeaponCur = index;
            ReadyOpenWeaponUI();
            GameManager.Instance.WeaponUIGoHomeUI();
        }
    }
    public void ReadyOpenWeaponUI()
    {
        for (int i = 0; i <= indexWeaponOpen; i++)
        {
            if (i < indexWeaponOpen)
            {
                weaponBuys[i].gameObject.SetActive(false);
            }   
            weaponUses[i].gameObject.SetActive(true);
            weaponUses[i].text = Constant.SELECT;
            GameManager.Instance.GetWeapons()[i].SetActive(false);
        }
        GameManager.Instance.GetWeapons()[indexWeaponCur].SetActive(true);
        weaponUses[indexWeaponCur].text = Constant.EQUIPPED;

        int length = GameManager.Instance.GetTabWeaponUI().Length;
        for (int i = 0; i < length; i++)
        {
            GameManager.Instance.GetTabWeaponUI()[i].SetActive(false);
        }
        GameManager.Instance.GetTabWeaponUI()[indexWeaponCur].SetActive(true);
    }
    public void BuyHair()
    {
        foreach(int i in hairsBought)
        {
            if (indexSkinChoose == i)
            {
                return;
            }
        }

        if (Constant.COSTSKIN <= GameManager.Instance.GetGold())
        {
            GameManager.Instance.SetGold(-Constant.COSTSKIN);
            GameManager.Instance.SetNumGoldText(GameManager.Instance.GetGold());
            hairsBought.Add(indexSkinChoose);
            SetBoughtSkin(hairBuy, hairUse);
        }
    }
    public void SetBoughtSkin(Text off, Text on)
    {
        off.gameObject.SetActive(false);
        on.gameObject.SetActive(true);
    }

    public void UseHair()
    {
        foreach (int i in hairsBought)
        {
            if (indexSkinChoose == i)
            {
                if (indexSkinChoose != hairCur)
                {
                    hairCur = indexSkinChoose;
                    SetHair(hairCur);
                    hairUse.text = Constant.UNEQUIP;
                }
                else
                {
                    hairCur = -1;
                    SetHair(hairCur);
                    hairUse.text = Constant.SELECT;
                }
            }
        }
    }

    public void ReadyOpenHairUI(int index)
    {
        if (index >= 0)
        {
            indexSkinChoose = index;
            foreach (int i in hairsBought)
                if (index == i)
                {

                    SetBoughtSkin(GetHairBuyText(), GetHairUseText());
                    break;
                }
                else
                    SetBoughtSkin(GetHairUseText(), GetHairBuyText());
        }

        if (indexSkinChoose != GetHairCur())
        {
            GetHairUseText().text = Constant.SELECT;
        }
        else
        {
            GetHairUseText().text = Constant.UNEQUIP;
        }
    }
    public void SetTextNamePlayer()
    {
        namePlayer.text = inputNamePlayer.text;
    }
    public void SaveSkinCur()
    {
        SavePantCur(pant.material);
        for (int i = 0; i < hairs.Length; i++)
        {
            if (hairs[i].activeSelf == true)
            {
                hairCur = i;
                break;
            }
        }
        for (int i = 0; i < shields.Length; i++)
        {
            if (shields[i].activeSelf == true)
            {
                SaveShieldCur(i);
                break;
            }
        }
    }
    public void SavePantCur(Material m)
    {
        pantCur = m;
    }
    public void SaveShieldCur(int i)
    {
        shieldCur = i;
    }
    public void SaveSetCur(int i)
    {
        setCur = i;
    }
    public void LoadSkinCur()
    {
        if (!isUseSet)
        {
            LoadSkinOrigin();
            pant.material = pantCur;
            if (hairCur < 0)
                SetHair(-1);
            else
                SetHair(hairCur);
            if (shieldCur < 0)
                SetShield(-1);
            else
                SetShield(shieldCur);
        }
        else
        {
            SetSet(setCur);
        }
    }
    public void LoadSkinOrigin()
    {
        pant.material = pantOrigin;
        body.material = bobyOrigin;
        SetHair(-1);
        SetShield(-1);
        foreach (GameObject itemSet in sets)
        {
            itemSet.SetActive(false);
        }
    }
    public void AutoSetChangeTab(int indexTab)
    {
        switch (indexTab)
        {
            case (int)SkinOrder.Hair:
                LoadSkinOrigin();
                if (hairCur < 0)
                {
                    SetHair(0);
                }
                else
                {
                    SetHair(hairCur);
                }
                break;
            case (int)SkinOrder.Pant:
                LoadSkinOrigin();
                SetPant(0);
                break;
            case (int)SkinOrder.Shield:
                LoadSkinOrigin();
                SetShield(0);
                break;
            case (int)SkinOrder.Set:
                LoadSkinOrigin();
                SetSet(0);
                break;
        }
    }
    public void SetPant(int index)
    {
        pant.material = GameManager.Instance.GetPant()[index];
    }
    public void SetHair(int index)
    {
        ReadyOpenHairUI(index);
        if (index >= 0 && index < GameManager.Instance.GetHair().Length)
        {
            foreach (GameObject hair in hairs)
            {
                hair.SetActive(false);
            }
            hairs[index].SetActive(true);
        }
        if (index < 0)
        {
            foreach (GameObject hair in hairs)
            {
                hair.SetActive(false);
            }
        }
    }
    public void SetShield(int index)
    {
        if (index >= 0 && index < GameManager.Instance.GetShield().Length)
        {
            foreach (GameObject shield in shields)
            {
                shield.SetActive(false);
            }
            shields[index].SetActive(true);
        }
        if (index < 0)
        {
            foreach (GameObject shield in shields)
            {
                shield.SetActive(false);
            }
        }
    }
    public void SetSet(int index)
    {
        switch (index) 
        {
            case 0:
                LoadSkinOrigin();
                pant.material = GameManager.Instance.GetBody()[3];
                body.material = GameManager.Instance.GetBody()[3];
                SetHair(3);
                sets[0].SetActive(true);
                sets[1].SetActive(true);
                break;
            case 1:
                LoadSkinOrigin();
                pant.material = GameManager.Instance.GetPant()[9];
                body.material = GameManager.Instance.GetBody()[5];
                SetHair(4);
                sets[2].SetActive(true);
                sets[3].SetActive(true);
                break;
        }
    }
}
