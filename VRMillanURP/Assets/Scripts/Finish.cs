using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private List<GameObject> sticks = new List<GameObject>();
   // [SerializeField] private Floating[] floatScript;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip yipeeSound;
    [SerializeField] private AudioClip calmYipeeSound;
    [SerializeField] private AudioSource eligePremioAudio;
    [SerializeField] private ParticleSystem confetti;

    //[SerializeField] private Transform player;
  

    [SerializeField] private AudioManager audioManager;

    private bool _ended = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_ended) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        _ended = true;


        audioManager.StartOutroFade(1f);

        StartCoroutine(PlayFinishSounds());

        

        for (int i = 0; i < sticks.Count; i++)
        {
            sticks[i].SetActive(false);
        }

      
    }

    private IEnumerator PlayFinishSounds()
    {
        // Reproducir el primero
        audioSource.clip = yipeeSound;
        audioSource.Play();

        // Esperar a que termine
        yield return new WaitForSeconds(yipeeSound.length - 0.5f);

        // Reproducir el segundo
        audioSource.clip = calmYipeeSound;
        audioSource.Play();
        eligePremioAudio.Play();
        confetti.Play();
    }

  
}