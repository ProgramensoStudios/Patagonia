using UnityEngine;

public class PoleTip : MonoBehaviour
{
    public bool isTouchingGround = false;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] impactSounds;

    private float lastTouchTime = -1f;
    private float touchBuffer = 0.15f; // tiempo que sigue contando como contacto

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            lastTouchTime = Time.time;

            if (impactSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, impactSounds.Length);
                audioSource.PlayOneShot(impactSounds[randomIndex]);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            lastTouchTime = Time.time;
        }
    }

    private void Update()
    {
        isTouchingGround = (Time.time - lastTouchTime) < touchBuffer;
    }
}
