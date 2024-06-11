using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Transform character;
    [SerializeField] bool isKillEnemy;
    [SerializeField] MeshRenderer meshWeapon;
    [SerializeField] BoxCollider colliWeapon;

    void OnEnable()
    {
        meshWeapon.enabled = true;
        colliWeapon.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character") && other.gameObject != character.gameObject)
        {
            meshWeapon.enabled = false;
            colliWeapon.enabled = false;
            transform.localScale *= 1.08f;
            character.localScale *= 1.08f;
            isKillEnemy = true;
            GameManager.Instance.NumberEnemyAlive();
        }
    }

    public void SetCharacter(Transform character)
    {
        this.character = character;
    }
    
    public GameObject getCharacter()
    {
        return character.gameObject;
    }


    public bool IsKillEnemy()
    {
        return isKillEnemy;
    }

    public void ResertIsKillEnemy()
    {
        isKillEnemy = false;
    }
}
