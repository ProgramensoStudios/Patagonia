using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour
{
    public enum IntroState
    {
        Darkness,
        LightingUp,
        Gameplay
    }

    [SerializeField] private AudioManager audioManager;

    public IntroState currentState;

    [Header("References")]
    [SerializeField] private LightingController lightingController;
    [SerializeField] private PlayerController playerController;

    [Header("Timing Settings")]
    [SerializeField] private float darknessDuration;
    [SerializeField] private float lightingUpDuration;
    [SerializeField] private float postFadeDelay;

    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        // ------------------
        // DARKNESS
        // ------------------
        currentState = IntroState.Darkness;

        playerController.enabled = false;
        lightingController.SetDarkness();

        yield return new WaitForSeconds(darknessDuration);

        // ------------------
        // LIGHTING UP
        // ------------------
        currentState = IntroState.LightingUp;

        lightingController.StartCrossFade(lightingUpDuration);

        if (audioManager != null)
            audioManager.StartIntroFade(lightingUpDuration);

        yield return new WaitForSeconds(lightingUpDuration);

        yield return new WaitForSeconds(postFadeDelay);

        // ------------------
        // GAMEPLAY
        // ------------------
        currentState = IntroState.Gameplay;
        playerController.enabled = true;

        Debug.Log("Gameplay iniciado");
    }
}