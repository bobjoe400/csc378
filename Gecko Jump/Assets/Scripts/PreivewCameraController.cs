using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PreivewCameraController : MonoBehaviour
{
    public PlayerInput playerInput;
    public float initialCameraSpeed = 0.2f; // Speed of the camera movement
    public float cameraReturnScalar = 2.0f; // Speed at which the camera returns to the player
    public float cameraHalfwayWait = 1.0f; // Time to wait when the camera reaches halfway

    private CinemachineSplineDolly dollyCamera;
    private CinemachineCamera cinemachineCamera;

    private SplineAutoDolly.FixedSpeed myFixedSpeed;


    void Start()
    {
        dollyCamera = GetComponent<CinemachineSplineDolly>();
        cinemachineCamera = GetComponent<CinemachineCamera>();

        myFixedSpeed = new SplineAutoDolly.FixedSpeed{ Speed = initialCameraSpeed }; // Initialize fixed speed for automatic dolly movement
        dollyCamera.AutomaticDolly.Method = myFixedSpeed;

        dollyCamera.AutomaticDolly.Enabled = true; // Enable automatic dolly movement
        playerInput.enabled = false; // Disable player input to prevent interference
    }

    void Update()
    {
        if (dollyCamera != null)
        {
            if (dollyCamera.CameraPosition > 1.0f)
            {
                dollyCamera.AutomaticDolly.Enabled = false; // Disable automatic dolly movement when past a certain point
                cinemachineCamera.Priority = 9; // Set camera priority to ensure it is active
                playerInput.enabled = true; // Re-enable player input for further interactions
            }
            else if (dollyCamera.CameraPosition > 0.5f)
            {
                myFixedSpeed.Speed = 0.0f;  // Stop the camera movement when reaching halfway
                StartCoroutine(WaitForCameraToFinish()); // Wait for a moment before re-enabling the camera

            }
        }
    }

    IEnumerator WaitForCameraToFinish()
    {   
        yield return new WaitForSeconds(cameraHalfwayWait); // Wait for 1 second before re-enabling player input
        myFixedSpeed.Speed = initialCameraSpeed * cameraReturnScalar; // Reset the camera speed
    }
}
