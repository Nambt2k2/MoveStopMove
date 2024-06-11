using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Transform character;
    [SerializeField] bool killEnemy;
    [SerializeField] MeshRenderer meshWeapon;
    [SerializeField] BoxCollider colliWeapon;

    void OnEnable()
    {
        meshWeapon.enabled = true;
        colliWeapon.enabled = true;
    }

    public void SetCharacter(Transform character)
    {
        this.character = character;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character") && other.gameObject != character.gameObject)
        {
            meshWeapon.enabled = false;
            colliWeapon.enabled = false;
            transform.localScale *= 1.2f;
            character.localScale *= 1.2f;
            killEnemy = true;
            GameManager.Instance.EnemyDie();
        }   
    }
    
    public bool GetKill()
    {
        return killEnemy;
    }

    public void ResetKill()
    {
        killEnemy = false;
    }
}
