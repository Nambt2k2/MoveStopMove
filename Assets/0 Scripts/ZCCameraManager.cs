using System.Collections;
using UnityEngine;

public class ZCCameraManager : MonoBehaviour
{
    Vector3 cameraOffset;
    [SerializeField] Transform player;
    //object fader
    ObjectFader fader;
    Ray ray;
    RaycastHit hit;
    Vector3 dir;

    void Awake()
    {
        cameraOffset = transform.position - player.position;
    }

    void Update()
    {
        dir = player.position - transform.position;
        ray = new Ray(transform.position, dir);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == null) return;
            if (hit.collider.gameObject == player.gameObject)
            {
                if (fader != null) fader.DoFade = false;
            }
            else
            {
                fader = hit.collider.gameObject.GetComponent<ObjectFader>();
                if (fader != null) fader.DoFade = true;
            }
        }
    }

    public void ShrinkCamera(int scale)
    {
        StartCoroutine(AnimateShrinkCamera(scale));
    }
    IEnumerator AnimateShrinkCamera(int scale)
    {
        float elapsed = 0f;
        float duration = 1f;
        Vector3 from = cameraOffset;
        Vector3 to = cameraOffset * scale;

        while (elapsed < duration)
        {
            cameraOffset = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraOffset = to;
    }
}
