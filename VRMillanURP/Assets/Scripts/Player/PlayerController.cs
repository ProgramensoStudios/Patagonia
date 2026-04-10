using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [Header("VR Head")]
    public Transform vrHead;

    [Header("Controllers")]
    public Transform leftController;
    public Transform rightController;

    [Header("Input")]
    public InputData inputData;

    [Header("Start Delay")]
    public float startMoveDelay = 5f;
    private bool canMove = false;

    [Header("Rhythm Detection")]
    public float armMoveThreshold = 0.04f;
    public float rhythmTimeout = 0.8f;

    [Header("Speed Settings")]
    public float minWalkSpeed;
    public float maxWalkSpeed;
    public float acceleration;
    public float deceleration;

    [Header("Bob Settings")]
    public float bobAmount;

    [Header("Height")]
    public float heightOffset = 0f;
    public float rotationSmooth = 4f;

    private float currentSpeed = 0f;
    private float targetSpeed = 0f;

    private Vector3 lastLeftPos;
    private Vector3 lastRightPos;

    private enum ArmSide { None, Left, Right }
    private ArmSide lastArm = ArmSide.None;

    private float lastStepTime = 0f;
    private float cadence = 0f;
    private float rhythmTimer = 0f;
    private float bobTimer = 0f;

    private void Start()
    {
        lastLeftPos = leftController.position;
        lastRightPos = rightController.position;

        StartCoroutine(StartMoveRoutine());
    }

    private IEnumerator StartMoveRoutine()
    {
        canMove = false;
        yield return new WaitForSeconds(startMoveDelay);
        canMove = true;
    }

    private void Update()
    {
        if (!canMove)
        {
            currentSpeed = 0f;
            targetSpeed = 0f;
            return;
        }

        bool grabbing = BothGrabsPressed();

        if (grabbing)
        {
            DetectArm(leftController, ref lastLeftPos, ArmSide.Left);
            DetectArm(rightController, ref lastRightPos, ArmSide.Right);
        }
        else
        {
            targetSpeed = 0f;
        }

        rhythmTimer += Time.deltaTime;

        if (rhythmTimer > rhythmTimeout)
        {
            targetSpeed = 0f;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * 3f * Time.deltaTime);
        }
        else
        {
            float smooth = targetSpeed > currentSpeed ? acceleration : deceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, smooth * Time.deltaTime);
        }

        if (currentSpeed < 0.1f)
            currentSpeed = 0f;

        MoveFree();
    }

    private bool BothGrabsPressed()
    {
        bool leftGrip = false;
        bool rightGrip = false;

        if (inputData._leftController.isValid)
            inputData._leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGrip);

        if (inputData._rightController.isValid)
            inputData._rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGrip);

        return leftGrip && rightGrip;
    }

    private void DetectArm(Transform controller, ref Vector3 lastPos, ArmSide side)
    {
        Vector3 delta = controller.position - lastPos;
        lastPos = controller.position;

        if (-delta.y < armMoveThreshold) return;
        if (side == lastArm) return;

        float now = Time.time;

        if (lastStepTime > 0f)
        {
            float stepInterval = now - lastStepTime;
            cadence = 1f / Mathf.Max(stepInterval, 0.1f);

            float normalized = Mathf.InverseLerp(1.0f, 2.5f, cadence);
            targetSpeed = Mathf.Lerp(minWalkSpeed, maxWalkSpeed, normalized);
        }

        lastStepTime = now;
        lastArm = side;
        rhythmTimer = 0f;
    }

    private void MoveFree()
    {
        if (Terrain.activeTerrain == null) return;

        Vector3 forward = vrHead.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 move = forward * currentSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + move;

        float terrainHeight = Terrain.activeTerrain.SampleHeight(newPos);
        terrainHeight += Terrain.activeTerrain.transform.position.y;

        if (currentSpeed > 0.05f && cadence > 0f)
        {
            bobTimer += Time.deltaTime * cadence * Mathf.PI;
            float bob = Mathf.Sin(bobTimer) * bobAmount;
            newPos.y = terrainHeight + heightOffset + bob;
        }
        else
        {
            bobTimer = 0f;
            newPos.y = terrainHeight + heightOffset;
        }

        transform.position = newPos;

        if (forward != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSmooth * Time.deltaTime
            );
        }
    }
}