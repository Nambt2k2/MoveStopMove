using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] DynamicJoystick joytick;
    [Header("Weapon")]
    [SerializeField] Transform holdWeapon, posWeapon;
    [SerializeField] GameObject weaponPrefab, weapon;
    [SerializeField] WeaponManager weaponController;
    [Header("Player")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] CapsuleCollider colli;
    [SerializeField] GameObject circleRangeAtk, infoPrefab, info;
    [SerializeField] bool isMove, inRangeAtk, canAtk;
    [SerializeField] int moveSpeed, level;
    [SerializeField] float angle, angleAtkRotation, rangeAtk;
    [SerializeField] Text namePlayer, textLevel;
    [SerializeField] Transform posInfo;
    [Header("PlayerSkin")]
    [SerializeField] SkinnedMeshRenderer body, pant;
    [SerializeField] Material pantCur, pantOrigin, bobyOrigin;
    [SerializeField] GameObject[] hairs, shields, sets;
    [SerializeField] GameObject hairBegin, shieldBegin;
    [SerializeField] int hairCur, shieldCur, setCur;
    [SerializeField] bool isUseSet;
    [Header("Enemys")]
    [SerializeField] float[] distances;
    [SerializeField] Vector3 posEnemy, directionEnemy;
    [Header("Camera")]
    [SerializeField] FollowCamera cam;
    [Header("Animation")]
    [SerializeField] PlayerAnimation anim;
    [SerializeField] StateAnimation stateAnim = StateAnimation.Idle;

    void Awake()
    {
        distances = new float[Constant.NUMCHARACTER1TURN];
        info = Instantiate(infoPrefab);
        namePlayer = info.GetComponentsInChildren<Text>()[0];
        textLevel = info.GetComponentsInChildren<Text>()[1];
        info.SetActive(false);
        namePlayer.text = "You";
        weapon = Instantiate(weaponPrefab);
        weapon.SetActive(false);
        weaponController = weapon.GetComponent<WeaponManager>();
        weaponController.SetCharacter(transform);
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
            GameManager.Instance.SetNumGold(level);
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

    //set,get
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

    //UI
    public void SaveSkinCur()
    {
        SavePantCur(pant.material);
        for (int i = 0; i < hairs.Length; i++)
        {
            if (hairs[i].activeSelf == true)
            {
                SaveHairCur(i);
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
    public void SaveHairCur(int i)
    {
        hairCur = i;
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
                SetHair(0);
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
                pant.material = GameManager.Instance.GetBody()[3];
                body.material = GameManager.Instance.GetBody()[3];
                SetHair(3);
                sets[0].SetActive(true);
                sets[1].SetActive(true);
                break;

        }
    }
}
