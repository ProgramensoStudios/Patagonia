using UnityEngine;

public class Floating : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;

    [Header("Rotation Settings")]
    public bool useSpin = false;
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 0f;

    [Header("Look At")]
    public Transform player;
    public bool onlyYAxis = true; // evita que se incline raro

    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        // -------- FLOAT --------
        float newY = _startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            _startPosition.x,
            newY,
            _startPosition.z
        );

        // -------- LOOK AT PLAYER --------
        if (player != null)
        {
            Vector3 lookPos = player.position;

            if (onlyYAxis)
                lookPos.y = transform.position.y;

            transform.LookAt(lookPos);
        }

        // -------- OPTIONAL SPIN --------
        if (useSpin)
        {
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }
}