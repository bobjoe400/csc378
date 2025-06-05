using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PreivewCameraController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float initialCameraSpeed = 0.2f;
    [SerializeField] private float cameraReturnScalar = 2.0f;
    [SerializeField] private float cameraStartWait = 0.2f;
    [SerializeField] private float cameraHalfwayWait = 1.0f;
    
    [Header("Skip UI")]
    [SerializeField] private PreviewUIController previewUiController; // Reference to your skip slider

    [SerializeField] private bool isMovingForward;

    private CinemachineSplineDolly dollyCamera;
    private CinemachineCamera cinemachineCamera;
    private SplineAutoDolly.FixedSpeed myFixedSpeed;

    void Start()
    {
        dollyCamera = GetComponent<CinemachineSplineDolly>();
        cinemachineCamera = GetComponent<CinemachineCamera>();

        myFixedSpeed = new SplineAutoDolly.FixedSpeed { Speed = initialCameraSpeed };
        dollyCamera.AutomaticDolly.Method = myFixedSpeed;

        StartCoroutine(WaitForCameraToStart());
    }

    void Update()
    {
        if (dollyCamera != null && dollyCamera.AutomaticDolly.Enabled)
        {
            if (isMovingForward)
            {
                if (dollyCamera.CameraPosition > 1.0f)
                {
                    StartCoroutine(WaitAtHalfway());
                }
            }
            else
            {
                if (dollyCamera.CameraPosition < 0.0f)
                {
                    CancelCamera();
                }
            }
        }
    }

    IEnumerator WaitForCameraToStart()
    {
        playerInput.enabled = false;

        // Show the skip UI and enable skipping
        if (previewUiController != null)
        {
            previewUiController.ShowSkipUI();
            previewUiController.OnSkipComplete += CancelCamera; // Subscribe to skip completion
        }

        yield return new WaitForSeconds(cameraStartWait);

        dollyCamera.AutomaticDolly.Enabled = true;
        cinemachineCamera.Priority = 11;
        myFixedSpeed.Speed = initialCameraSpeed;

        
        isMovingForward = true;
    }

    IEnumerator WaitAtHalfway()
    {   
        myFixedSpeed.Speed = 0.0f;
        yield return new WaitForSeconds(cameraHalfwayWait);

        isMovingForward = false;
        myFixedSpeed.Speed = -1 * initialCameraSpeed * cameraReturnScalar;
    }
    void CancelCamera()
    {
        dollyCamera.AutomaticDolly.Enabled = false;
        playerInput.enabled = true;
        cinemachineCamera.Priority = 9;

        // Hide the skip UI when camera movement is complete
        if (previewUiController != null)
        {
            previewUiController.HideSkipUI();
            previewUiController.OnSkipComplete -= CancelCamera;
        }
    }
}