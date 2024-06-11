using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform targetObject;
    [SerializeField] Vector3 cameraOffset;
    Camera cam;

    [SerializeField] Image[] dirEnemys;

    void Awake()
    {
        cameraOffset = transform.position - targetObject.position;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        transform.position = targetObject.position + cameraOffset;
        if (!GameManager.Instance.GetIsGameOver())
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
        Vector2 posPlayer = cam.WorldToScreenPoint(GameManager.Instance.GetPosEnemy()[0].position);

        for (int i = 1; i < 10; i++)
        {
            Vector2 posDirEnemy = cam.WorldToScreenPoint(GameManager.Instance.GetPosEnemy()[i].position);
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
        StartCoroutine(Animate());
    }
    IEnumerator Animate()
    {
        float elapsed = 0f;
        float duration = 1f;
        Vector3 from = cameraOffset;
        Vector3 to = cameraOffset * 1.05f;

        while (elapsed < duration)
        {
            cameraOffset = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraOffset = to;
    }
}
