using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform targetObject;
    [SerializeField] Camera camCheckEnemy, camFollowPlayer;
    [SerializeField] Animator camFollowAnimator, camCheckEnemyAnimator;
    [SerializeField] Vector3 cameraOffset;

    [SerializeField] Image[] dirEnemys;

    void Awake()
    {
        cameraOffset = transform.position - targetObject.position;
    }

    void LateUpdate()
    {
        transform.position = targetObject.position + cameraOffset;
        if (!GameManager.Instance.GetIsGameOver() && GameManager.Instance.GetIsStartGame())
        {
            ArrowFollowPosEnemy();
        } 
    }

    void ArrowFollowPosEnemy()
    {
        float minX = dirEnemys[0].GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = dirEnemys[0].GetPixelAdjustedRect().width / 2;
        float maxY = Screen.height - minY;
        Vector2 posPlayer = camCheckEnemy.WorldToScreenPoint(GameManager.Instance.GetPosEnemy()[0].position);

        for (int i = 1; i < 10; i++)
        {
            Vector2 posDirEnemy = camCheckEnemy.WorldToScreenPoint(GameManager.Instance.GetPosEnemy()[i].position);
            Vector2 dir = posDirEnemy - posPlayer;
            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            dirEnemys[i - 1].transform.eulerAngles = Vector3.back * angle;
            if (posDirEnemy.x < minX || posDirEnemy.x > maxX || posDirEnemy.y < minY || posDirEnemy.y > maxY)
            {
                GameManager.Instance.GetEnemy()[i - 1].SetArrowSelf(true);
            }
            else
            {
                GameManager.Instance.GetEnemy()[i - 1].SetArrowSelf(false);
            }

            posDirEnemy.x = Mathf.Clamp(posDirEnemy.x, minX, maxX);
            posDirEnemy.y = Mathf.Clamp(posDirEnemy.y, minY, maxY);
            dirEnemys[i - 1].transform.position = posDirEnemy;  
        }
    }

    public void ShrinkCamera()
    {
        StartCoroutine(AnimateShrinkCamera());
    }
    IEnumerator AnimateShrinkCamera()
    {
        float elapsed = 0f;
        float duration = 1f;
        Vector3 from = cameraOffset;
        Vector3 to = cameraOffset * Constant.SHRINKCAM;

        while (elapsed < duration)
        {
            cameraOffset = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraOffset = to;
    }

    public void MoveCameraToPlayGame()
    {
        camFollowAnimator.enabled = true;
        camCheckEnemyAnimator.enabled = true;
    }

    public void MoveCameraToSkinUI(int dirTime)
    {
        StartCoroutine(AnimateRotationCamera(dirTime));
    }
    IEnumerator AnimateRotationCamera(int dirTime)
    {
        float elapsed = 0f;
        float duration = 1f;
        float from = 43 - dirTime * 5;
        float to = from + 10 * dirTime;

        while (elapsed < duration)
        {
            transform.localEulerAngles = Vector3.right * Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = Vector3.right * to;
    }
    public Camera GetCamCheckEnemy()
    {
        return camCheckEnemy;
    }
}
