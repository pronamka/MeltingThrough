using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VisualEffectManager : MonoBehaviour
{
    [Header("Post-Processing")]
    public Volume postProcessVolume;

    [Header("Blur Effect")]
    public Material blurMaterial;
    public Camera mainCamera;

    private RenderTexture blurTexture;
    private Coroutine blurCoroutine;

    
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private DepthOfField depthOfField;

    public static VisualEffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out colorAdjustments);
            postProcessVolume.profile.TryGet(out depthOfField);
        }

        SetupBlurEffect();
    }

    private void SetupBlurEffect()
    {
        if (blurMaterial == null)
        {
            
            Shader blurShader = Shader.Find("Custom/EdgeBlur");
            if (blurShader != null)
            {
                blurMaterial = new Material(blurShader);
            }
        }
    }

    public void ApplyEdgeBlur(float intensity, float edgeWidth, AnimationCurve falloff)
    {
        if (blurMaterial != null)
        {
            blurMaterial.SetFloat("_BlurIntensity", intensity);
            blurMaterial.SetFloat("_EdgeWidth", edgeWidth);

            
            if (mainCamera != null)
            {
                EdgeBlurEffect blurEffect = mainCamera.GetComponent<EdgeBlurEffect>();
                if (blurEffect == null)
                {
                    blurEffect = mainCamera.gameObject.AddComponent<EdgeBlurEffect>();
                }
                blurEffect.Initialize(blurMaterial, intensity, edgeWidth, falloff);
            }
        }
    }

    public void RemoveEdgeBlur()
    {
        if (mainCamera != null)
        {
            EdgeBlurEffect blurEffect = mainCamera.GetComponent<EdgeBlurEffect>();
            if (blurEffect != null)
            {
                blurEffect.RemoveEffect();
            }
        }
    }

    
    public void ApplyColorFilter(Color filterColor, float intensity)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = Color.Lerp(Color.white, filterColor, intensity);
            colorAdjustments.active = true;
        }
    }

    public void ApplyVignette(float intensity, float smoothness)
    {
        if (vignette != null)
        {
            vignette.intensity.value = intensity;
            vignette.smoothness.value = smoothness;
            vignette.active = true;
        }
    }

    public void RemoveAllEffects()
    {
        RemoveEdgeBlur();

        if (vignette != null)
        {
            vignette.active = false;
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.active = false;
        }
    }
}