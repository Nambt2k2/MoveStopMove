using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerManager player;
    [SerializeField] bool isAtk;
    [SerializeField] float timeAtk, timeAtkReturn;
    WeaponSO weaponData;


    void Update()
    {
        if (isAtk && timeAtk < Constant.TIMEATK)
        {
            if (weaponData.typeAtk == WeaponManager.TypeAtk.Straight)
            {
                player.AtkStraight();
                timeAtk += Time.deltaTime;
            }
            else if (weaponData.typeAtk == WeaponManager.TypeAtk.Rotation)
            {
                player.AtkRotation();
                timeAtk += Time.deltaTime;
            }
        }
        else
        {
            isAtk = false;
            timeAtk = 0;
        }

        if (isAtk && timeAtkReturn < Constant.TIMEATK * 2)
        {
            if (weaponData.typeAtk == WeaponManager.TypeAtk.Return)
            {
                player.AtkReturn();
                timeAtkReturn += Time.deltaTime;
            }
            if (player.SqrMagnitudeWeaponToPlayer() < 0.05)
            {
                isAtk = false;
                timeAtkReturn = 0;
                player.GetWeapon().SetActive(false);
            }
        }
        else
        {
            isAtk = false;
            timeAtkReturn = 0;
        }
    }
    public void LoadWeaponData()
    {
        weaponData = GameManager.Instance.GetDataPlayer().GetWeaponData(player.GetIndexWeaponCur());
    }

    public void UpdateAnimation(StateAnimation stateAnim)
    {
        for (int i = 0; i <= (int)StateAnimation.Dead; i++)
        {
            string nameState = ((StateAnimation) i).ToString();
            if (stateAnim == (StateAnimation) i)
            {
                animator.SetBool(nameState, true);
            }
            else
            {
                animator.SetBool(nameState, false);
            }
        }  
    }

    public void SetFalseCanAtk()
    {
        player.SetFalseCanAtk();
    }
    public void SetFalseDiplayWeapon()
    {
        player.SetFalseDiplayWeapon();
        isAtk = true;
    }
    public void SetTrueDiplayWeapon()
    {
        player.SetTrueDiplayWeapon();
    }
}
