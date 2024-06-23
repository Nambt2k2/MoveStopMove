using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [Header("Weapon-----------")]
    [SerializeField] Transform holdWeapon, posWeapon;
    [SerializeField] GameObject weaponPrefab, weapon;
    [SerializeField] WeaponManager weaponController;
    [Header("Enemy------------")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] CapsuleCollider colli;
    [SerializeField] SpriteRenderer isPlayerChooseAtk;
    [SerializeField] GameObject info, infoPrefab;
    [SerializeField] bool isMove, inRangeAtk, canAtk;
    [SerializeField] int moveSpeed, level;
    [SerializeField] float angle, angleAtkRotation, rangeAtk;
    [SerializeField] Text nameEnemy, textLevel;
    [SerializeField] Transform posInfo;
    [SerializeField] Image arrowSelf;
    [Header("EnemySkin---------")]
    [SerializeField] SkinnedMeshRenderer body, pant;
    [Header("Enemys------------")]
    [SerializeField] float[] distances;
    [SerializeField] Vector3 posEnemy, directionEnemy;
    [Header("Animation---------")]
    [SerializeField] EnemyAnimation anim;
    [SerializeField] StateAnimation stateAnim = StateAnimation.Idle;

    void Awake()
    {
        distances = new float[Constant.NUMCHARACTER1TURN];
        info = Instantiate(infoPrefab);
        nameEnemy = info.GetComponentsInChildren<Text>()[0];
        textLevel = info.GetComponentsInChildren<Text>()[1];
        info.SetActive(false);
        weapon = Instantiate(weaponPrefab);
        weapon.SetActive(false);
        weaponController = weapon.GetComponent<WeaponManager>();
        weaponController.SetCharacter(transform);
    }

    void Start()
    {
        RandomSkinEnemy();
    }

    void Update()
    {
        if (GameManager.Instance.IsStartGame)
        {
            Move();
            CheckRangeAtk();
            if (!isMove && inRangeAtk && holdWeapon.gameObject.activeSelf && !weapon.activeSelf)
            {
                canAtk = true;
            }
        }
        
        UpdateState();
        anim.UpdateAnimation(stateAnim);
        
        if (GameManager.Instance.IsGameOver || !GameManager.Instance.IsStartGame)
        {
            arrowSelf.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (weaponController.IsKillEnemy())
        {
            rangeAtk *= Constant.ZOOMLEVELUP;
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
            enabled = false;
            rigid.velocity = Vector3.zero;
            colli.enabled = false;
            info.SetActive(false);
            posInfo.localPosition =  Vector3.up;
            anim.UpdateAnimation(StateAnimation.Dead);
        }
    }

    void Move()
    {
        info.transform.position = posInfo.position;
        if (!inRangeAtk && GameManager.Instance.Alive > 1)
        {
            directionEnemy = posEnemy - transform.position;
            angle = Mathf.Atan2(directionEnemy.x, directionEnemy.z) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * angle;
            transform.position += Time.deltaTime * moveSpeed * directionEnemy.normalized;
            isMove = true;
            stateAnim = StateAnimation.Run;
        }
        else
        {
            isMove = false;
            stateAnim = StateAnimation.Idle;
        }
    }

    //spawn
    public void Spwan()
    {
        gameObject.SetActive(true);
        enabled = true;
        colli.enabled = true;
        info.SetActive(true);
        arrowSelf.gameObject.SetActive(true);
        RandomSkinEnemy();
        transform.position = new Vector3(Random.Range(-15, 15), 50.1f, Random.Range(-15, 15));
        transform.localScale = Constant.CHARACTERLOCALSCALEBEGIN * Vector3.one;
        weapon.transform.localScale = Constant.WEAPONLOCALSCALEBEGIN * Vector3.one;
        rangeAtk = Constant.RANGEATKBEGIN;
        level = 0;
        textLevel.text = level.ToString();
    }
    void RandomSkinEnemy()
    {
        body.material = GameManager.Instance.ColorBodys[Random.Range(0, GameManager.Instance.ColorBodys.Length)];
        pant.material = GameManager.Instance.ColorPants[Random.Range(0, GameManager.Instance.ColorPants.Length)];
    }

    //atk
    void CheckRangeAtk()
    {
        for (int i = 0; i < 10; i++)
        {
            if (GameManager.Instance.Characters[i].activeSelf && GameManager.Instance.ColliCharacter[i].enabled)
            {
                distances[i] = (GameManager.Instance.PosEnemy[i].position - transform.position).sqrMagnitude;
            }
            else
            {
                distances[i] = Constant.DISTANCEWHENDIE;
            }
        }

        float min = distances[0];

        for (int i = 1; i < 10; i++)
        {
            if (distances[i] != 0)
            {
                min = distances[i];
                break;
            }
        }

        for (int i = 0; i < 10; i++)
        {
            if (min > distances[i] && distances[i] != 0)
            {
                min = distances[i];
            }
        }

        if (min < rangeAtk * rangeAtk)
        {
            inRangeAtk = true;
            for (int i = 0; i < 10; i++)
            {
                if (min == distances[i])
                {
                    posEnemy = GameManager.Instance.PosEnemy[i].position;
                }
            }
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                if (min == distances[i])
                {
                    posEnemy = GameManager.Instance.PosEnemy[i].position;
                    break;
                }
            }
            inRangeAtk = false;
            canAtk = false;
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
    public void IsPlayerChooseAtk(bool b)
    {
        isPlayerChooseAtk.enabled = b;
    }

    //animation
    void UpdateState()
    {
        if (canAtk)
            stateAnim = StateAnimation.Attack;
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
    public void SetArrowSelf(bool b)
    {
        arrowSelf.gameObject.SetActive(b);
    }
    
    //get,set
    public string GetNameEnemy()
    {
        return nameEnemy.text;
    }
    public void SetNameEnemy(string s)
    {
        nameEnemy.text = s;
    }
    public GameObject GetInfo()
    {
        return info;
    }
}
