using UnityEngine;
using UnityEngine.Rendering;

public class ObjectFader : MonoBehaviour
{
    Material mat;
    bool doFade;
    public bool DoFade
    {
        get
        {
            return doFade;
        }
        set
        {
            doFade = value;
        }
    }

    void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    void Update()
    {
        if (doFade)
        {
            mat.SetInt("_SrcBlend", (int) BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int) BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int) RenderQueue.Transparent;
            Color c = mat.color;
            c = new Color(c.r, c.g, c.b, 0.3f);
            mat.color = c;
        }
        else
        {
            mat.SetInt("_SrcBlend", (int) BlendMode.One);
            mat.SetInt("_DstBlend", (int) BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = -1;
            Color c = mat.color;
            c = new Color(c.r, c.g, c.b, 1f);
            mat.color = c;
        }
    }
}
