using UnityEngine;
using System.Collections.Generic;

public class RaycastFoliageSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> prefabs = new List<GameObject>();

    [Header("Spawn Settings")]
    public int spawnCount = 100;
    public Vector2 areaSize = new Vector2(50f, 50f);

    [Header("Raycast Settings")]
    public float raycastStartHeight = 500f;
    public float raycastDistance = 1000f;
    public LayerMask groundLayer;

    [Header("Rotation Settings")]
    public bool alignToSurface = true;
    public bool randomYRotation = true;

    void Start()
    {
        SpawnFoliage();
    }

    void SpawnFoliage()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            if (prefabs.Count == 0)
            {
                Debug.LogWarning("No prefabs assigned!");
                return;
            }

            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];

            float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
            float randomZ = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);

            Vector3 rayOrigin = new Vector3(
                transform.position.x + randomX,
                raycastStartHeight,
                transform.position.z + randomZ
            );

            Ray ray = new Ray(rayOrigin, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
            {
                Vector3 spawnPosition = hit.point;

                // Base rotation
                Quaternion rotation = Quaternion.identity;

                // Align to slope
                if (alignToSurface)
                {
                    rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }

                // Random Y rotation
                if (randomYRotation)
                {
                    rotation *= Quaternion.Euler(0, Random.Range(0f, 360f), 0f);
                }

                Instantiate(prefab, spawnPosition, rotation);
            }
            else
            {
                //Debug.LogWarning("Raycast did not hit ground. Check groundLayer.");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(areaSize.x, 0.1f, areaSize.y)
        );
        
        Gizmos.color = Color.red;

        Vector3 rayStart = new Vector3(
            transform.position.x,
            transform.position.y + raycastStartHeight,
            transform.position.z
        );

        Vector3 rayEnd = rayStart + Vector3.down * raycastDistance;

       // Gizmos.DrawLine(rayStart, rayEnd);
    }

}
