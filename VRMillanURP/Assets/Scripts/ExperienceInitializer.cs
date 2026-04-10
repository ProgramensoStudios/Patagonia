using UnityEngine;
using UnityEngine.XR;
using System.Collections;

public class ExperienceInitializer : MonoBehaviour
{
    public PoleCalibrator poleCalibrator;
    public InputData inputData;

    [Header("Calibration Settings")]
    public float holdTimeRequired = 3f;
    public float vibrationMaxStrength = 0.6f;

    private float holdTimer = 0f;
    private bool calibrated = false;
    private bool isCalibrating = false;

    private Coroutine calibrationRoutine;

    public System.Action OnCalibrationComplete;

    private void Update()
    {
        if (!isCalibrating) return;
        if (calibrated) return;
        if (inputData == null) return;

        bool leftPressed = false;
        bool rightPressed = false;

        if (inputData._leftController.isValid)
            inputData._leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftPressed);

        if (inputData._rightController.isValid)
            inputData._rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightPressed);

        if (leftPressed && rightPressed)
        {
            holdTimer += Time.deltaTime;

            float progress = holdTimer / holdTimeRequired;
            float vibration = Mathf.Lerp(0.1f, vibrationMaxStrength, progress);

            SendHaptics(vibration);

            if (holdTimer >= holdTimeRequired && calibrationRoutine == null)
            {
                calibrationRoutine = StartCoroutine(CalibrateSequence());
            }
        }
        else
        {
            holdTimer = 0f;
            SendHaptics(0f);
        }
    }

    // ESTE es el método que llama GameState
    public void StartCalibration()
    {
        Debug.Log(">>> EXPERIENCE INITIALIZER START CALLED");

        StopAllCoroutines();

        calibrated = false;
        isCalibrating = true;
        holdTimer = 0f;

        SendHaptics(0f);
    }


    private IEnumerator CalibrateSequence()
    {
        calibrated = true;
        isCalibrating = false;

        SendHaptics(0f);

        poleCalibrator.Calibrate();

        yield return StartCoroutine(DoublePulse());

        Debug.Log("Calibración completa");

        OnCalibrationComplete?.Invoke();

        calibrationRoutine = null;
    }

    private IEnumerator DoublePulse()
    {
        SendHaptics(0.8f);
        yield return new WaitForSeconds(0.08f);

        SendHaptics(0f);
        yield return new WaitForSeconds(0.08f);

        SendHaptics(0.8f);
        yield return new WaitForSeconds(0.08f);

        SendHaptics(0f);
    }

    private void ResetCalibration()
    {
        calibrated = false;
        isCalibrating = false;
        holdTimer = 0f;

        SendHaptics(0f);

        if (calibrationRoutine != null)
        {
            StopCoroutine(calibrationRoutine);
            calibrationRoutine = null;
        }

        Debug.Log("Calibración reseteada");
    }

    private void SendHaptics(float intensity)
    {
        if (inputData._leftController.isValid)
            inputData._leftController.SendHapticImpulse(0, intensity, 0.1f);

        if (inputData._rightController.isValid)
            inputData._rightController.SendHapticImpulse(0, intensity, 0.1f);
    }
}
