using UnityEngine;
using Unity.Cinemachine;

public class SpawnGerald : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private GameObject geraldPrefab; // Assign this in the inspector or load from Resources

    void Start()
    {
        CreateGerald();
    }

    void CreateGerald()
    {
        // Find the GameObject with the tag "Gerald"
        GameObject gerald = GameObject.FindGameObjectWithTag("Player");

        // If not found, instantiate a new one
        if (gerald == null)
        {
            Debug.Log("Gerald not found, spawning a new one.");
            GameObject newGerald = Instantiate(geraldPrefab, transform.position, Quaternion.identity);
            newGerald.transform.rotation = Quaternion.Euler(0, 0, -90); // Set rotation if needed

            cinemachineCamera.Target.TrackingTarget = newGerald.transform;
        }
        else
        {
            Debug.Log("Gerald already exists in the scene.");
        }
    }
}