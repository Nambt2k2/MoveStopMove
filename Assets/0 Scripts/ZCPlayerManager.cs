using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZCPlayerManager : MonoBehaviour
{
    [Header("Weapon---------")]
    int indexWeaponCur;
    bool isAtkReturnPlayer;
    [SerializeField] Transform holdWeapon, posWeapon;
    [SerializeField] GameObject weapon;
    [SerializeField] ZCWeaponManager weaponController;
    [SerializeField] GameObject[] weaponPrefab;
    [SerializeField] MeshRenderer[] holdWeapons;
    [Header("Player---------")]
    bool isMove, inRangeAtk, canAtk, isDie;
    int level, goldBeforeDie;
    Vector3 directionPlayer;
    [SerializeField] float moveSpeed;
    [SerializeField] float angle, angleAtkRotation, rangeAtk;
    [SerializeField] CapsuleCollider colli;
    [SerializeField] GameObject circleRangeAtk, infoPrefab, info;
    [SerializeField] Transform posInfo;
    [Header("PlayerUI--------")]
    [SerializeField] Text namePlayer, textLevel;
    [Header("PlayerSkin------")]
    [SerializeField] int pantCur, hairCur, shieldCur, setCur;
    [SerializeField] SkinnedMeshRenderer body, pant;
    [SerializeField] Material pantOrigin, bobyOrigin;
    [SerializeField] GameObject[] hairs, shields, sets;
    [SerializeField] Material[] pants, bodys;
    [Header("Enemys----------")]
    ZCZombieManager zombie;
    List<GameObject> zombies;
    [SerializeField] GameObject isChooseAtkZombie;
    [SerializeField] Vector3 posEnemy, directionEnemy;
    [SerializeField] List<float> distances;
    [Header("Camera----------")]
    [SerializeField] ZCCameraManager cam;
    [Header("Animation-------")]
    [SerializeField] ZCPlayerAnimation anim;
    [SerializeField] Transform avatar;
    [SerializeField] StateAnimation stateAnim = StateAnimation.Idle;
    [Header("Data------------")]
    [SerializeField] DataPlayer data;
    public WeaponSO weaponData { get; private set; }

    void Awake()
    {
        distances = new List<float>();
        info = Instantiate(infoPrefab);
        info.transform.position = posInfo.position;
        namePlayer = info.GetComponentsInChildren<Text>()[0];
        textLevel = info.GetComponentsInChildren<Text>()[1];

        indexWeaponCur = data.LoadGame().IndexWeaponCur;
        hairCur = data.LoadGame().IndexHairCur;
        pantCur = data.LoadGame().IndexPantCur;
        shieldCur = data.LoadGame().IndexShieldCur;
        setCur = data.LoadGame().IndexSetCur;
        weaponData = data.GetWeaponData(indexWeaponCur);
        namePlayer.text = data.LoadGame().NamePlayer;

        UseSkin();
        if (GameObject.FindGameObjectWithTag(Constant.WEAPONDATA) != null)
        {
            UseWeapon(GameObject.FindGameObjectWithTag(Constant.WEAPONDATA).GetComponent<MeshRenderer>().materials);
        }  
    }

    void Start()
    {
        zombies = ObjectPool.Instance.PooledObjects[(int)ObjectPool.ObjectInPool.Zombie];
        SpawnWeaponPlayer();
    }

    void Update()
    {
        Move(directionPlayer);
        SetDir(directionPlayer);
        info.transform.position = posInfo.position;
        CheckRangeAtk();
        if (!isMove && inRangeAtk && holdWeapon.gameObject.activeSelf && !weapon.activeSelf)
        {
            canAtk = true;
            RotaionPlayerAtk();
        }

        UpdateState();
        anim.UpdateAnimation(stateAnim);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Constant.ZOMBIE))
        {
            isDie = true;
            enabled = false;
            anim.UpdateAnimation(StateAnimation.Dead);
            foreach (GameObject z in zombies)
            {
                if (z.activeInHierarchy)
                {
                    zombie = z.GetComponent<ZCZombieManager>();
                    zombie.Agent.enabled = false;
                    zombie.enabled = false;
                    zombie.Anim.SetBool(ZCZombieManager.StateAnimationZombie.Win.ToString(), true);
                }
            }
        }
    }

    void Move(Vector3 direction)
    {
        directionPlayer.x = JoystickCustom.Horizontal();
        directionPlayer.z = JoystickCustom.Vertical();

        if (direction.x != 0 || direction.z != 0)
        {
            transform.Translate(moveSpeed * Time.deltaTime * direction);
            isMove = true;
        }
        else
        {
            isMove = false;
        }
    }
    public void SetDir(Vector3 direction)
    {
        if (direction.x != 0 || direction.z != 0)
        {
            angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            avatar.eulerAngles = Vector3.up * angle;
        }
    }

    public void RevivePlayer()
    {
        transform.position = new Vector3(Random.Range(-15, 15), 50.1f, Random.Range(-15, 15));
        enabled = true;
        colli.enabled = true;
        circleRangeAtk.SetActive(true);
    }

    //atk
    void CheckRangeAtk()
    {
        for (int i = 0; i < zombies.Count; i++)
        {
            if (distances.Count < zombies.Count && distances.Count <= i)
            {
                if (zombies[i].activeSelf)
                {
                    distances.Add((zombies[i].transform.position - transform.position).sqrMagnitude);
                }
                else
                {
                    distances.Add(Constant.DISTANCEWHENDIE);
                }
            }
            else
            {
                if (zombies[i].activeSelf)
                {
                    distances[i] = (zombies[i].transform.position - transform.position).sqrMagnitude;
                }
                else
                {
                    distances[i] = Constant.DISTANCEWHENDIE;
                }
            }
            
        }

        float min = distances[0];

        for (int i = 1; i < zombies.Count; i++)
        {
            if (min > distances[i])
            {
                min = distances[i];
            }
        }

        if (min > rangeAtk * rangeAtk)
        {
            inRangeAtk = false;
            canAtk = false;
            isChooseAtkZombie.SetActive(false);
        }

        for (int i = 0; i < zombies.Count; i++)
        {
            isChooseAtkZombie.SetActive(false);
            if (min == distances[i] && min < rangeAtk * rangeAtk)
            {
                inRangeAtk = true;
                posEnemy = zombies[i].transform.position;
                isChooseAtkZombie.transform.position = posEnemy;
                isChooseAtkZombie.SetActive(true);
                break;
            }
        }
    }
    public void AtkRotation()
    {
        if (!weapon.activeSelf)
        {
            directionEnemy = posEnemy - transform.position;
            weapon.transform.position = posWeapon.position;
            weapon.SetActive(true);
        }
        weapon.transform.Rotate(Vector3.forward * angleAtkRotation * Time.deltaTime);
        weapon.transform.position += directionEnemy.normalized * Time.deltaTime * rangeAtk * Constant.DISTANCEWEAPONALIVE;
    }
    public void AtkStraight()
    {
        if (!weapon.activeSelf)
        {
            directionEnemy = posEnemy - transform.position;
            weapon.transform.eulerAngles = Vector3.back * angle;
            weapon.transform.position = posWeapon.position;
            weapon.SetActive(true);
        }
        weapon.transform.position += directionEnemy.normalized * Time.deltaTime * rangeAtk * Constant.DISTANCEWEAPONALIVE;
    }
    public void AtkReturn()
    {
        if (!weapon.activeSelf)
        {
            isAtkReturnPlayer = false;
            directionEnemy = posEnemy - transform.position;
            weapon.transform.position = posWeapon.position;
            weapon.SetActive(true);
        }
        if (Vector3.SqrMagnitude(posWeapon.position - weapon.transform.position) > rangeAtk * rangeAtk * 1.2f)
        {
            isAtkReturnPlayer = true;
        }
        if (isAtkReturnPlayer)
        {
            directionEnemy = transform.position - weapon.transform.position;
        }
        weapon.transform.Rotate(Vector3.forward * angleAtkRotation * Time.deltaTime);
        weapon.transform.position += directionEnemy.normalized * Time.deltaTime * rangeAtk * Constant.DISTANCEWEAPONALIVE * 1.1f;
    }

    void RotaionPlayerAtk()
    {
        directionEnemy = posEnemy - transform.position;
        angle = Mathf.Atan2(directionEnemy.x, directionEnemy.z) * Mathf.Rad2Deg;
        avatar.eulerAngles = Vector3.up * angle;
    }
    public void SpawnWeaponPlayer()
    {
        weapon = Instantiate(weaponPrefab[indexWeaponCur]);
        weapon.SetActive(false);
        weaponController = weapon.GetComponent<ZCWeaponManager>();
        weaponController.PosPlayer = transform;
        weaponController.Player = this;
    }
    public float SqrMagnitudeWeaponToPlayer()
    {
        return (transform.position - weapon.transform.position).sqrMagnitude;
    }

    //animation
    void UpdateState()
    {
        if (isMove)
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
    public GameObject CircleRangeAtk
    {
        get
        {
            return circleRangeAtk;
        }
    }
    public GameObject Info
    {
        get
        {
            return info;
        }
    }
    public StateAnimation StateAnim
    {
        set
        {
            stateAnim = value;
        }
    }
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
        }
    }
    public GameObject Weapon
    {
        get
        {
            return weapon;
        }
    }
    public bool IsMove
    {
        get
        {
            return isMove;
        }
    }
    public bool IsDie
    {
        get
        {
            return isDie;
        }
    }
    public int TextLevel
    {
        set
        {
            textLevel.text = value.ToString();
        }
    }

    //UI
    void UseSkin()
    {
        foreach (GameObject hair in hairs)
            hair.SetActive(false);
        foreach (GameObject shield in shields)
            shield.SetActive(false);
        foreach (GameObject set in sets)
            set.SetActive(false);
        pant.material = pantOrigin;
        body.material = bobyOrigin;

        if (hairCur > 0)
        {
            hairs[hairCur].SetActive(true);
            return;
        }
        if (pantCur > 0)
        {
            pant.material = pants[pantCur];
            return;
        }
        if (shieldCur > 0)
        {
            shields[shieldCur].SetActive(true);
            return;
        }
        if (setCur < 0)
        {
            return;
        }
        else if (setCur == 0)
        {
            pant.material = bodys[3];
            body.material = bodys[3];
            hairs[3].SetActive(true);
            sets[0].SetActive(true);
            sets[1].SetActive(true);
        }
        else if (setCur == 1)
        {
            pant.material = bodys[5];
            body.material = bodys[5];
            hairs[4].SetActive(true);
            sets[2].SetActive(true);
            sets[3].SetActive(true);
        }     
    }

    void UseWeapon(Material[] m)
    {
        foreach (MeshRenderer weapon in holdWeapons)
        {
            weapon.gameObject.SetActive(false);
        }
        holdWeapons[indexWeaponCur].gameObject.SetActive(true);
        holdWeapons[indexWeaponCur].materials = m;
        weaponPrefab[indexWeaponCur].GetComponent<MeshRenderer>().materials = m;
    }
}

   