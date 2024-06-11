using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerManager player;
    [SerializeField] bool isAtk;
    [SerializeField] float timeAtk;

    void Update()
    {
        if (isAtk && timeAtk < 1)
        {
            timeAtk += Time.deltaTime;
            player.AtkRotation();
        }
        else
        {
            isAtk = false;
            player.ResetWeapon();
            timeAtk = 0;
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
