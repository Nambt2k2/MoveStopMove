using UnityEngine;

public class ZCPlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] ZCPlayerManager player;
    [SerializeField] bool isAtk;
    [SerializeField] float timeAtk, timeAtkReturn;

    void Update()
    {
        if (isAtk && timeAtk < Constant.TIMEATK)
        {
            if (player.weaponData.typeAtk == TypeAtk.Straight)
            {
                player.AtkStraight();
                timeAtk += Time.deltaTime;
            }
            else if (player.weaponData.typeAtk == TypeAtk.Rotation)
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
            if (player.weaponData.typeAtk == TypeAtk.Return)
            {
                player.AtkReturn();
                timeAtkReturn += Time.deltaTime;
            }
            if (player.SqrMagnitudeWeaponToPlayer() < 0.05)
            {
                isAtk = false;
                timeAtkReturn = 0;
                player.Weapon.SetActive(false);
            }
        }
        else
        {
            isAtk = false;
            timeAtkReturn = 0;
        }
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
