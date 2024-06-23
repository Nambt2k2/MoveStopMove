using UnityEngine;
using System.Collections;

public class ZCWeaponManager : MonoBehaviour
{
    [SerializeField] float timeLife;
    [SerializeField] Transform posPlayer;
    [SerializeField] ZCPlayerManager player;
    [SerializeField] MeshRenderer meshWeapon;
    [SerializeField] BoxCollider colliWeapon;
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
        if (other.CompareTag(Constant.ZOMBIE))
        {
            meshWeapon.enabled = false;
            colliWeapon.enabled = false;
            other.gameObject.SetActive(false);
            player.Level = 1;
            player.TextLevel = player.Level;
        }
    }

    IEnumerator DeactiveAfterTime()
    {
        yield return new WaitForSeconds(timeLife);
        gameObject.SetActive(false);
    }

    public Transform PosPlayer
    {
        set
        {
            posPlayer = value;
        }
    }
    public ZCPlayerManager Player
    {
        set
        {
            player = value;
        }
    }
}
