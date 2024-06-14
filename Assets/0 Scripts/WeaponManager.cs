using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Transform character;
    [SerializeField] bool isKillEnemy;
    [SerializeField] MeshRenderer meshWeapon;
    [SerializeField] BoxCollider colliWeapon;
    [SerializeField] float timeLife;
    Coroutine deactiveWait = null;

    void OnEnable()
    {
        meshWeapon.enabled = true;
        colliWeapon.enabled = true;
        deactiveWait = StartCoroutine(DeactiveAfterTime());
    }
    void OnDisable()
    {
        if (deactiveWait != null)
        {
            StopCoroutine(deactiveWait);
            deactiveWait = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constant.CHARACTER) && other.gameObject != character.gameObject)
        {
            meshWeapon.enabled = false;
            colliWeapon.enabled = false;
            transform.localScale *= Constant.ZOOMLEVELUP;
            character.localScale *= Constant.ZOOMLEVELUP;
            isKillEnemy = true;
            GameManager.Instance.NumberEnemyAlive();
        }
    }

    IEnumerator DeactiveAfterTime()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
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
