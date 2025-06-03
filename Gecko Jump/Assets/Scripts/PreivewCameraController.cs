using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PreivewCameraController : MonoBehaviour
{
    public PlayerInput playerInput;
    public float initialCameraSpeed = 0.2f; // Speed of the camera movement
    public float cameraReturnScalar = 2.0f; // Speed at which the camera returns to the player
    public float cameraStartWait = 0.2f; // Time to wait before starting the camera movement
    public float cameraHalfwayWait = 1.0f; // Time to wait when the camera reaches halfway

    [SerializeField] private bool forward;

    private CinemachineSplineDolly dollyCamera;
    private CinemachineCamera cinemachineCamera;

    private SplineAutoDolly.FixedSpeed myFixedSpeed;


    void Start()
    {
        dollyCamera = GetComponent<CinemachineSplineDolly>();
        cinemachineCamera = GetComponent<CinemachineCamera>();

        myFixedSpeed = new SplineAutoDolly.FixedSpeed { Speed = initialCameraSpeed }; // Initialize fixed speed for automatic dolly movement
        dollyCamera.AutomaticDolly.Method = myFixedSpeed;

        StartCoroutine(WaitForCameraToStart()); // Start the camera movement after a delay
    }

    void Update()
    {
        if (dollyCamera != null)
        {
            if (forward)
            {
                if (dollyCamera.CameraPosition > 1.0f)
                {
                    myFixedSpeed.Speed = 0.0f; // Stop the camera when it reaches the end
                    forward = false; // Reverse direction when reaching the end
                    StartCoroutine(WaitForCameraToFinish()); // Wait before re-enabling player input
                }
            }
            else
            {
                if (dollyCamera.CameraPosition <= 0.0f)
                {
                    dollyCamera.AutomaticDolly.Enabled = false; // Enable automatic dolly movement
                    playerInput.enabled = true; // Re-enable player input when the camera returns to the start
                    cinemachineCamera.Priority = 9; // Reset camera priority
                }
            }
        }
    }

    IEnumerator WaitForCameraToStart()
    {
        yield return new WaitForSeconds(cameraStartWait); // Optional delay before starting the camera movement

        dollyCamera.AutomaticDolly.Enabled = true; // Enable automatic dolly movement
        playerInput.enabled = false; // Disable player input while the camera is moving
        cinemachineCamera.Priority = 11; // Set camera priority for the preview camera
        myFixedSpeed.Speed = initialCameraSpeed; // Start moving the camera forward

        InputSystem.onAnyButtonPress.CallOnce(CancelCamera); // Listen for any input to cancel the camera movement

        forward = true; // Set the direction to forward
    }

    IEnumerator WaitForCameraToFinish()
    {
        yield return new WaitForSeconds(cameraHalfwayWait); // Wait for 1 second before re-enabling player input
        myFixedSpeed.Speed = -1 * initialCameraSpeed * cameraReturnScalar; // Reset the camera speed
    }

    void CancelCamera(InputControl control)
    {
        forward = false; // Disable forward movement when any input is detected
        dollyCamera.CameraPosition = 0.0f; // Reset camera position to the start
    }
}
