using System;
using UnityEngine;
using UnityEngine.XR;

public class Prize : MonoBehaviour
{
    public static Action<GameObject> OnSelectedPrize;
    public static Action OnPrize;

    [Header("Haptic Settings")]
    [SerializeField] private InputData inputData;
    [SerializeField] private Transform rightControllerTransform;
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private float maxDistance = 2.5f;
    [SerializeField] private float minStrength = 0.05f;
    [SerializeField] private float maxStrength = 0.7f;

    [Header("Attach Settings")]
    [SerializeField] private Vector3 localPositionOffset;
    [SerializeField] private Vector3 localRotationOffset;

    [SerializeField] private AudioSource selectionSound;
    [SerializeField] private GameObject luz;

    private float timer;
    [NonSerialized] public bool isHaptic = false;
    private bool prizeChosen = false;

    private bool rightHandInside = false;
    private bool leftHandInside = false;

    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if (prizeChosen || inputData == null || !isHaptic)
            return;

        if (!(rightHandInside && leftHandInside))
            return;

        float rightDistance = Vector3.Distance(rightControllerTransform.position, transform.position);
        float leftDistance = Vector3.Distance(leftControllerTransform.position, transform.position);

        float closestDistance = Mathf.Min(rightDistance, leftDistance);

        if (closestDistance > maxDistance)
            return;

        float normalized = 1f - Mathf.Clamp01(closestDistance / maxDistance);

        float dynamicInterval = Mathf.Lerp(0.3f, 0.05f, normalized);
        float strength = Mathf.Lerp(minStrength, maxStrength, normalized);

        timer += Time.deltaTime;

        if (timer >= dynamicInterval)
        {
            if (rightDistance < leftDistance)
                inputData._rightController.SendHapticImpulse(0, strength, 0.05f);
            else
                inputData._leftController.SendHapticImpulse(0, strength, 0.05f);

            timer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RightHand"))
            rightHandInside = true;

        if (other.CompareTag("LeftHand"))
            leftHandInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RightHand"))
            rightHandInside = false;

        if (other.CompareTag("LeftHand"))
            leftHandInside = false;
    }

    public void GrabPrize()
    {
        if (prizeChosen) return;
        if (!(rightHandInside && leftHandInside)) return;

        prizeChosen = true;

        // Elegir mano más cercana
        float rightDistance = Vector3.Distance(rightControllerTransform.position, transform.position);
        float leftDistance = Vector3.Distance(leftControllerTransform.position, transform.position);

        Transform chosenHand = rightDistance < leftDistance ? rightControllerTransform : leftControllerTransform;

        // Attach
        transform.SetParent(chosenHand);

        // Reset local transform para que se pegue bonito
        transform.localPosition = localPositionOffset;
        transform.localRotation = Quaternion.Euler(localRotationOffset);

        // Desactivar físicas para evitar jitter
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (col != null)
        {
            col.enabled = false;
        }

        // Feedback
        OnSelectedPrize?.Invoke(gameObject);
        OnPrize?.Invoke();
        selectionSound.Play();

        var firework = GetComponentInChildren<ParticleSystem>();
        if (firework != null)
            firework.Play();

        // luz.SetActive(false);
    }
}