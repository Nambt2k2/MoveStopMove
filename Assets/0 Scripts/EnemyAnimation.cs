using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] EnemyManager enemy;
    [SerializeField] bool isAtk;
    [SerializeField] float timeAtk;

    void Update()
    {
        if (isAtk && timeAtk < 1)
        {
            timeAtk += Time.deltaTime;
            enemy.AtkRotation();
        }
        else
        {
            isAtk = false;
            enemy.ResetWeapon();
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
        enemy.SetFalseCanAtk();
    }
    public void SetFalseDiplayWeapon()
    {
        enemy.SetFalseDiplayWeapon();
        isAtk = true;
    }
    public void SetTrueDiplayWeapon()
    {
        enemy.SetTrueDiplayWeapon();
    }
    public void SetFalseActive()
    {
        enemy.gameObject.SetActive(false);
    }
}
