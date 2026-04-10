using System.Collections;
using UnityEngine;

public class LightingController : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Light calibrationSpotLight;

    [Header("Sun Settings")]
    [SerializeField] private float originalSunIntensity;
    [SerializeField] private float calibrationSpotIntensity;


    [Header("Fade")]


    [SerializeField]  private float fadeTimer = 0f;
    [SerializeField]  private float fadeDuration;
    private bool isCrossFading = false;

    private void Awake()
    {
        if (directionalLight != null)
            originalSunIntensity = directionalLight.intensity;
    }

    public void SetDarkness()
    {
        if (directionalLight != null)
            directionalLight.intensity = 0f;

        RenderSettings.ambientLight = Color.black;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        if (calibrationSpotLight != null)
        {
            calibrationSpotLight.enabled = true;
            calibrationSpotLight.intensity = calibrationSpotIntensity;
        }
    }

    public void StartCrossFade(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(CrossFadeRoutine(duration));
    }

    private IEnumerator CrossFadeRoutine(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            float smoothT = t * t * (3f - 2f * t);

            if (directionalLight != null)
                directionalLight.intensity = Mathf.Lerp(0f, originalSunIntensity, smoothT);

            if (calibrationSpotLight != null)
                calibrationSpotLight.intensity = Mathf.Lerp(calibrationSpotIntensity, 0f, smoothT);

            yield return null;
        }

        if (directionalLight != null)
            directionalLight.intensity = originalSunIntensity;

        if (calibrationSpotLight != null)
            calibrationSpotLight.enabled = false;
    }

    public bool UpdateCrossFade()
    {
        if (!isCrossFading) return false;

        fadeTimer += Time.deltaTime;
        float t = fadeTimer / fadeDuration;

        // SmoothStep manual (más orgánico)
        float smoothT = t * t * (3f - 2f * t);

        if (directionalLight != null)
            directionalLight.intensity = Mathf.Lerp(0f, originalSunIntensity, smoothT);

        if (calibrationSpotLight != null)
            calibrationSpotLight.intensity = Mathf.Lerp(calibrationSpotIntensity, 0f, smoothT);

        if (t >= 1f)
        {
            isCrossFading = false;

            if (calibrationSpotLight != null)
                calibrationSpotLight.enabled = false;

            return true;
        }

        return false;
    }
}
