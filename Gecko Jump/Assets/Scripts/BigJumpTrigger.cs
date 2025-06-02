using UnityEngine;
using Unity.Cinemachine;

public class BigJumpTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public CinemachineCamera bigJumpCamera;

    [SerializeField] private float waitPeriod = 1.0f;
    [SerializeField] private float resetScalar = 2.0f;

    [SerializeField] private bool isPlayerInTrigger = false;
    [SerializeField] private float totalWait = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInTrigger)
        {
            if (totalWait < waitPeriod)
            {
                totalWait += Time.deltaTime;
            }
            else
            {
                totalWait = waitPeriod; // Keep at max
                bigJumpCamera.Priority = 11; // Set camera priority for big jump
            }
        }
        else
        {
            if (totalWait > 0.0f)
            {
                totalWait -= resetScalar * Time.deltaTime; // Decrease wait time if player is not in trigger
            }
            else
            {
                if (bigJumpCamera.Priority > 9)
                {
                    bigJumpCamera.Priority = 9; // Reset camera priority if player exits
                }
                totalWait = 0.0f; // Ensure totalWait does not go negative
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) isPlayerInTrigger = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) isPlayerInTrigger = false;
    }
}
