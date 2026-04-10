using System.Collections;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private ParticleSystem prizeParticles;

    [Header("Fade Settings")]
    [SerializeField] private float duration = 3f;

    private float initialIntensity;
    private Coroutine currentLerp;

    private void Awake()
    {
        if (directionalLight != null)
            initialIntensity = directionalLight.intensity;
    }

    private void OnEnable()
    {
        Prize.OnPrize += HandlePrize;
    }

    private void OnDisable()
    {
        Prize.OnPrize -= HandlePrize;
    }

    private void HandlePrize()
    {
        // Activa partículas justo cuando se gana
        if (prizeParticles != null)
            prizeParticles.Play();

        FadeToZero();
    }

    public void FadeToZero()
    {
        StartFade(directionalLight.intensity, 35.36079f);
    }

    public void FadeToInitial()
    {
        StartFade(directionalLight.intensity, initialIntensity);
    }

    private void StartFade(float from, float to)
    {
        if (currentLerp != null)
            StopCoroutine(currentLerp);

        currentLerp = StartCoroutine(LerpLightIntensity(from, to, duration));
    }

    private IEnumerator LerpLightIntensity(float startValue, float endValue, float time)
    {
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time;

            // Ease suave cinematográfico
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            directionalLight.intensity = Mathf.Lerp(startValue, endValue, smoothT);
            yield return null;
        }

        directionalLight.intensity = endValue;
    }
}