using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AdjustablePole : XRGrabInteractable
{
    [Header("References")]
    public Transform poleVisual;

    [Header("Input")]
    public InputActionProperty triggerAction;

    [Header("Settings")]
    public float minLength = 0.7f;
    public float maxLength = 1.6f;
    public float thickness = 1f;

    private bool isScaling = false;
    private float startHandHeight;
    private float startPoleLength;

    private Transform currentHand;
    public bool IsGrabbed { get; private set; }

    private void Start()
    {
        IsGrabbed = false;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        currentHand = args.interactorObject.transform;
        IsGrabbed = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        isScaling = false;
        currentHand = null;
        IsGrabbed = false;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            UpdateScaling();
        }
    }

    void UpdateScaling()
    {
        if (currentHand == null)
            return;

        float triggerValue = triggerAction.action.ReadValue<float>();

        if (triggerValue > 0.1f)
        {
            if (!isScaling)
            {
                isScaling = true;
                startHandHeight = currentHand.position.y;
                startPoleLength = poleVisual.localScale.z;
            }

            float deltaHeight = currentHand.position.y - startHandHeight;

            float newLength = startPoleLength + deltaHeight;
            newLength = Mathf.Clamp(newLength, minLength, maxLength);

            ApplyLength(newLength);
        }
        else
        {
            isScaling = false;
        }
    }

    void ApplyLength(float length)
    {
        poleVisual.localScale = new Vector3(thickness, thickness, length);
        poleVisual.localPosition = new Vector3(0f, 0f, -length * 0.5f);
    }
}
