using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Ambient")]
    [SerializeField] private AudioSource ambientSource;

    [Header("Wind")]
    [SerializeField] private AudioSource windSource;
    [SerializeField] private float minHeight = 0f;
    [SerializeField] private float maxHeight = 100f;
    [SerializeField] private float maxWindVolume = 1f;

    [Header("Bird")]
    [SerializeField] private AudioSource birdSource;
    [SerializeField] private AudioClip birdClip;
    [SerializeField] private float minBirdDelay = 5f;
    [SerializeField] private float maxBirdDelay = 15f;

    [Header("Intro Fade")]
    [SerializeField] private float introVolumeMultiplier = 0.05f;

    [Header("Global Volume Limit")]
    [SerializeField] private float maxGlobalVolume; // VOLUMEN MAXIMO REAL

    [SerializeField]  private float ambientBaseVolume;
    [SerializeField]  private float windBaseVolume;
    [SerializeField] private float birdBaseVolume;

    private float masterVolumeMultiplier = 0f;

    private void Start()
    {
        ambientBaseVolume = ambientSource.volume;
        windBaseVolume = windSource.volume;
        birdBaseVolume = birdSource.volume;

        ambientSource.loop = true;
        windSource.loop = true;

        masterVolumeMultiplier = introVolumeMultiplier;
        ApplyVolumes();

        ambientSource.Play();
        windSource.Play();

        StartCoroutine(BirdRoutine());
        StartIntroFade(3f);
    }

    private void Update()
    {
        UpdateWindVolume();
    }

    private void UpdateWindVolume()
    {
        float height = player.position.y;

        float normalizedHeight = Mathf.InverseLerp(minHeight, maxHeight, height);
        float windHeightVolume = normalizedHeight * maxWindVolume;

        float finalMultiplier = Mathf.Clamp01(masterVolumeMultiplier) * maxGlobalVolume;
        float finalWind = windHeightVolume * finalMultiplier;

        windSource.volume = Mathf.Lerp(windSource.volume, finalWind, Time.deltaTime * 2f);
    }

    private void ApplyVolumes()
    {
        float finalMultiplier = Mathf.Clamp01(masterVolumeMultiplier) * maxGlobalVolume;

        ambientSource.volume = ambientBaseVolume * finalMultiplier;
        birdSource.volume = birdBaseVolume * finalMultiplier;
    }

    public void SetMasterVolume(float value)
    {
        masterVolumeMultiplier = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void StartIntroFade(float duration)
    {
        StartCoroutine(IntroFadeRoutine(duration));
    }

    private IEnumerator IntroFadeRoutine(float duration)
    {
        float timer = 0f;
        float start = masterVolumeMultiplier;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            masterVolumeMultiplier = Mathf.Lerp(start, 1f, t);
            ApplyVolumes();

            yield return null;
        }

        masterVolumeMultiplier = 1f; // 1f ahora significa 0.7 real
        ApplyVolumes();
    }

    private IEnumerator BirdRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minBirdDelay, maxBirdDelay);
            yield return new WaitForSeconds(delay);

            if (birdClip != null)
            {
                birdSource.pitch = Random.Range(0.95f, 1.05f);
                birdSource.PlayOneShot(birdClip);
            }
        }
    }

    public void StartOutroFade(float duration)
    {
        StartCoroutine(OutroFadeRoutine(duration));
    }

    private IEnumerator OutroFadeRoutine(float duration)
    {
        float timer = 0f;
        float start = masterVolumeMultiplier;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            masterVolumeMultiplier = Mathf.Lerp(start, 0f, t);
            ApplyVolumes();

            yield return null;
        }

        masterVolumeMultiplier = 0f;
        ApplyVolumes();

        ambientSource.Stop();
        windSource.Stop();
    }
}