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
    [SerializeField] GameObject circleRangeAtk, infoPrefab ,info;
    [SerializeField] bool isMove, inRangeAtk, canAtk;
    [SerializeField] int moveSpeed, level;
    [SerializeField] float angle, angleAtkRotation, rangeAtk;
    [SerializeField] Text namePlayer, textLevel;
    [SerializeField] Transform posInfo;
    [Header("Enemys")]
    [SerializeField] float[] disdistances;
    [SerializeField] Vector3 posEnemy, directionEnemy;
    [Header("Camera")]
    [SerializeField] FollowCamera cam;
    [Header("Animation")]
    [SerializeField] PlayerAnimation anim;
    [SerializeField] StateAnimation stateAnim = StateAnimation.Idle;

    void Awake()
    {
        disdistances = new float[10];
        info = Instantiate(infoPrefab);
        namePlayer = info.GetComponentsInChildren<Text>()[0];
        textLevel = info.GetComponentsInChildren<Text>()[1];
        namePlayer.text = "Nam";
        weapon = Instantiate(weaponPrefab);
        weapon.SetActive(false);
        weaponController = weapon.GetComponent<WeaponManager>();
        weaponController.SetCharacter(transform);
    }

    void Update()
    {
        Move();
        CheckRangeAtk();

        if (!isMove && inRangeAtk && holdWeapon.gameObject.activeSelf && !weapon.activeSelf)
        {
            canAtk = true;
        }
        
        UpdateState();
        anim.UpdateAnimation(stateAnim);
    }

    void FixedUpdate()
    {
        
        if (weaponController.IsKillEnemy())
        {
            rangeAtk *= 1.08f;
            cam.ShrinkCamera();
            posInfo.localPosition *= 1.2f;
            weaponController.ResertIsKillEnemy();
            level++;
            textLevel.text = level.ToString();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && other.gameObject != weapon)
        {
            for (int i = 0; i < 9; i++)
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
            stateAnim = StateAnimation.Run;
        }
        else
        {
            isMove = false;
            stateAnim = StateAnimation.Idle;
        }
    }

    //atk
    void CheckRangeAtk()
    {
        for (int i = 0; i < 10; i++)
        {
            if (GameManager.Instance.GetCharacter()[i].activeSelf && GameManager.Instance.GetColliCharacter()[i].enabled)
            {
                disdistances[i] = (GameManager.Instance.GetPosEnemy()[i].position - transform.position).sqrMagnitude;
            }
            else
            {
                canAtk = false;
                disdistances[i] = 1000;
            }
        }

        float min = 0;

        for (int i = 0; i < 10; i++)
        {
            if (disdistances[i] != 0)
            {
                min = disdistances[i];
                break;
            }
        }
        
        for (int i = 0; i < 10; i++)
        {
            if (min > disdistances[i] && disdistances[i] != 0)
            {
                min = disdistances[i];
            }
        }

        if (min > rangeAtk * rangeAtk)
        {
            inRangeAtk = false;
            canAtk = false;
        }

        for (int i = 1; i < 10; i++)
        {
            GameManager.Instance.GetEnemy()[i - 1].IsPlayerChooseAtk(false);
            if (min == disdistances[i] && min < rangeAtk * rangeAtk)
            {
                inRangeAtk = true;
                posEnemy = GameManager.Instance.GetPosEnemy()[i].position;
                GameManager.Instance.GetEnemy()[i - 1].IsPlayerChooseAtk(true);
            }
        }

        if (min == 0)
        {
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
        weapon.transform.position += directionEnemy.normalized * Time.deltaTime * rangeAtk * 1.6f;
    }
    public void ResetWeapon()
    {
        weapon.SetActive(false);
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
}
