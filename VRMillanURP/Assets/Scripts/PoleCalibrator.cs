using UnityEngine;

public class PoleCalibrator : MonoBehaviour
{
    [Header("Controllers")]
    public Transform leftController;
    public Transform rightController;

    [Header("Pole Roots")]
    public Transform leftPoleRoot;
    public Transform rightPoleRoot;

    [Header("Pole Visuals (scale 1,1,1)")]
    public Transform leftPoleVisual;
    public Transform rightPoleVisual;

    [Header("Distance Settings")]
    public float minPoleLength = 0.6f;
    public float maxPoleLength = 2.2f;
    public float lengthMultiplier = 1f;
    public LayerMask groundLayer;

    private float leftMeshHeight;
    private float rightMeshHeight;

    void Awake()
    {
        leftMeshHeight = GetMeshHeight(leftPoleVisual);
        rightMeshHeight = GetMeshHeight(rightPoleVisual);

        // Forzamos escala limpia
        leftPoleRoot.localScale = Vector3.one;
        rightPoleRoot.localScale = Vector3.one;

        leftPoleVisual.localScale = Vector3.one;
        rightPoleVisual.localScale = Vector3.one;
    }

    float GetMeshHeight(Transform visual)
    {
        MeshFilter mf = visual.GetComponent<MeshFilter>();
        if (mf == null) return 1f;

        return mf.sharedMesh.bounds.size.y;
    }

    public void Calibrate()
    {
        CalibratePole(leftController, leftPoleRoot, leftPoleVisual, leftMeshHeight);
        CalibratePole(rightController, rightPoleRoot, rightPoleVisual, rightMeshHeight);

        Debug.Log("Poles calibrated clean (1,1,1 mode)");
    }

    void CalibratePole(Transform controller, Transform root, Transform visual, float meshHeight)
    {
        if (!Physics.Raycast(controller.position, Vector3.down, out RaycastHit hit, 5f, groundLayer))
            return;

        float distance = controller.position.y - hit.point.y;
        distance = Mathf.Clamp(distance, minPoleLength, maxPoleLength);
        distance *= lengthMultiplier;

        //  Root sigue la mano
        root.position = controller.position;
        root.rotation = controller.rotation;

        // Escalamos SOLO en Y
        float scaleY = distance / meshHeight;

        visual.localScale = new Vector3(
            1f,
            scaleY,
            1f
        );

        //  Bajamos el visual para tocar suelo
        visual.localPosition = new Vector3(
            0f,
            -distance / 2f,
            0f
        );
    }
}
