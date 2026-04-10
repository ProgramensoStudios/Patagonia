using UnityEngine;

public class MidExperience : MonoBehaviour
{
    [SerializeField] private AudioSource midMillanAudio;
    private void OnTriggerEnter(Collider other)
    {
        midMillanAudio.Play();
    }
}
