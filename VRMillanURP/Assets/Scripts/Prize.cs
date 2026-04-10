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

    [SerializeField] private AudioSource selectionSound;
    [SerializeField] private GameObject luz;

    private float timer;
    [NonSerialized] public bool isHaptic = false;
    private bool prizeChosen = false;

    private void Update()
    {
        if (prizeChosen || inputData == null || !isHaptic)
            return;

        // Measure distance to both controllers
        float rightDistance = Vector3.Distance(rightControllerTransform.position, transform.position);
        float leftDistance = Vector3.Distance(leftControllerTransform.position, transform.position);

        // Use the closer controller
        float closestDistance = Mathf.Min(rightDistance, leftDistance);

        if (closestDistance > maxDistance)
            return;

        float normalized = 1f - Mathf.Clamp01(closestDistance / maxDistance);

        float dynamicInterval = Mathf.Lerp(0.3f, 0.05f, normalized);
        float strength = Mathf.Lerp(minStrength, maxStrength, normalized);

        timer += Time.deltaTime;

        if (timer >= dynamicInterval)
        {
            // Vibrate the closer controller
            if (rightDistance < leftDistance)
                inputData._rightController.SendHapticImpulse(0, strength, 0.05f);
            else
                inputData._leftController.SendHapticImpulse(0, strength, 0.05f);

            timer = 0f;
        }
    }

   public void GrabPrize()
   {
           if(prizeChosen) return;
           prizeChosen = true;
           OnSelectedPrize?.Invoke(gameObject);
           OnPrize?.Invoke();
           selectionSound.Play();
        var firework = GetComponentInChildren<ParticleSystem>();
        firework.Play();
          // luz.SetActive(false);
        
    }
}