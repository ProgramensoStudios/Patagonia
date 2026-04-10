using System.Collections;
using UnityEngine;

public class InstructionsAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Start Instructions")]
    [SerializeField] private AudioClip[] startClips;
    [SerializeField] private float startDelay = 3f;

    [Header("Moment Instructions")]
    [SerializeField] private AudioClip[] momentClips;

    [SerializeField] private bool isGameStart;

    private bool _isPlayingSequence = false;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (isGameStart)
            StartCoroutine(PlayStartInstructions());
    }

    private IEnumerator PlayStartInstructions()
    {
        yield return new WaitForSeconds(startDelay);
        yield return StartCoroutine(PlayClipSequence(startClips));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isPlayingSequence) return;

        StartCoroutine(PlayClipSequence(momentClips));
    }

    private IEnumerator PlayClipSequence(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            yield break;

        _isPlayingSequence = true;

        for (int i = 0; i < clips.Length; i++)
        {
            audioSource.clip = clips[i];
            audioSource.Play();

            yield return new WaitForSeconds(clips[i].length);
        }

        _isPlayingSequence = false;
    }
}