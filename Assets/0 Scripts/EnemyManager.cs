using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] Transform holdWeapon, posWeapon;
    [SerializeField] GameObject weaponPrefab, weapon;
    [SerializeField] WeaponManager weaponController;
    [Header("Enemy")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] CapsuleCollider colli;
    [SerializeField] SpriteRenderer isChoose;
    [SerializeField] GameObject info, infoPrefab;
    [SerializeField] bool isMove, inRangeAtk, canAtk;
    [SerializeField] int moveSpeed, level;
    [SerializeField] float angle, angleAtkRotation, rangeAtk;
    [SerializeField] Text nameEnemy, textLevel;
    [SerializeField] Transform posInfo;
    [SerializeField] Image arrowSelf;
    [Header("Enemys")]
    [SerializeField] float[] disdistances;
    [SerializeField] Vector3 posEnemy, directionEnemy;
    [Header("Animation")]
    [SerializeField] EnemyAnimation anim;
    [SerializeField] StateAnimation stateAnim = StateAnimation.Idle;

    void Awake()
    {
        disdistances = new float[10];
        info = Instantiate(infoPrefab);
        nameEnemy = info.GetComponentsInChildren<Text>()[0];
        textLevel = info.GetComponentsInChildren<Text>()[1];
        nameEnemy.text = "Enemy";
        weapon = Instantiate(weaponPrefab);
        weapon.SetActive(false);
        weaponController = weapon.GetComponent<WeaponManager>();
        weaponController.SetCharacter(transform);
    }

    void Update()
    {
        CheckRangeAtk();
        Move();

        if (!isMove && inRangeAtk && holdWeapon.gameObject.activeSelf && !weapon.activeSelf)
        {
            canAtk = true;
        }

        UpdateState();
        anim.UpdateAnimation(stateAnim);
    }

    void FixedUpdate()
    {
        if (weaponController.GetKill())
        {
            rangeAtk *= 1.08f;
            posInfo.localPosition *= 1.2f;
            weaponController.ResetKill();
            level++;
            textLevel.text = level.ToString();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon") && other.gameObject != weapon)
        {
            enabled = false;
            rigid.velocity = Vector3.zero;
            colli.enabled = false;
            info.SetActive(false);
            posInfo.localPosition = new Vector3(0, 1, 0);
            anim.UpdateAnimation(StateAnimation.Dead);
        }
    }

    void Move()
    {
        info.transform.position = posInfo.position;
        if (!inRangeAtk)
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
        info.SetActive(true);
        anim.gameObject.SetActive(true);
        arrowSelf.gameObject.SetActive(true);
        transform.position = new Vector3(Random.Range(-15, 15), 50, Random.Range(-15, 15));
        enabled = true;
        colli.enabled = true;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        weapon.transform.localScale = new Vector3(19.5f, 19.5f, 19.5f);
        rangeAtk = 3.5f;
        level = 0;
        textLevel.text = level.ToString();
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
                disdistances[i] = 0;
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

        if (min < rangeAtk * rangeAtk)
        {
            inRangeAtk = true;
            for (int i = 0; i < 10; i++)
            {
                if (min == disdistances[i])
                {
                    posEnemy = GameManager.Instance.GetPosEnemy()[i].position;
                }
            }
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                if (min == disdistances[i])
                {
                    posEnemy = GameManager.Instance.GetPosEnemy()[i].position;
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
        weapon.transform.position += directionEnemy.normalized * Time.deltaTime * rangeAtk * 1.6f;
    }
    public void IsChoose(bool b)
    {
        isChoose.enabled = b;
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
    public void SetFalseDisplayAim()
    {
        anim.gameObject.SetActive(false);
    }
    public void SetArrowSelf(bool b)
    {
        arrowSelf.gameObject.SetActive(b);
    }
}
