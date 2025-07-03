using UnityEngine;

[RequireComponent(typeof(Camera))]
public class EdgeBlurEffect : MonoBehaviour
{
    private Material blurMaterial;
    private float blurIntensity;
    private float edgeWidth;
    private AnimationCurve falloffCurve;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void Initialize(Material material, float intensity, float width, AnimationCurve falloff)
    {
        blurMaterial = material;
        blurIntensity = intensity;
        edgeWidth = width;
        falloffCurve = falloff;

        enabled = true;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (blurMaterial != null && enabled)
        {
            
            blurMaterial.SetFloat("_BlurIntensity", blurIntensity);
            blurMaterial.SetFloat("_EdgeWidth", edgeWidth);
            blurMaterial.SetTexture("_MainTex", source);

            
            Graphics.Blit(source, destination, blurMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    public void RemoveEffect()
    {
        enabled = false;
        if (blurMaterial != null)
        {
            DestroyImmediate(blurMaterial);
        }
        DestroyImmediate(this);
    }
}