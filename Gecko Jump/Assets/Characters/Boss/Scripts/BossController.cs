using System.Collections;
using JetBrains.Annotations;
using Unity.Cinemachine;
using UnityEditor.Rendering;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Leading Settings")]
    public float predictionTime = 0.5f;        // How far ahead to predict
    public float maxLeadDistance = 10f;        // Maximum leading distance
    public float smoothingSpeed = 5f;          // How smoothly to update

    [Header("Target")]
    public Transform player;
    public Vector2 leadingTarget;            // Empty GameObject to represent leading position

    private Rigidbody2D playerRb;
    private Vector3 currentLeadingPos;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float soundVolume = 1f;

    private ProjectileLauncher fireBallLauncher;

    Animator animator;

    private bool isDead = false;

    void Start()
    {
        fireBallLauncher = GetComponent<ProjectileLauncher>();
        playerRb = player.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentLeadingPos = player.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newLeadingPos = CalculateLeadingPosition();

        // Smooth the leading position
        currentLeadingPos = Vector3.Lerp(currentLeadingPos, newLeadingPos, smoothingSpeed * Time.deltaTime);

        // Update the leading target transform
        leadingTarget = currentLeadingPos;

        DrawLeadingTargetSquare(leadingTarget, Color.yellow);
    }

    public void StartAttacking()
    {
        StartCoroutine(AttackPeriod());
    }

    IEnumerator AttackPeriod()
    {
        while (!isDead)
        {
            animator.SetBool("Attacking", true);
            yield return new WaitForSeconds(0.2f);

            animator.SetBool("Attacking", false);
            yield return new WaitForSeconds(3);
        }
    }

    Vector3 CalculateLeadingPosition()
    {
        // Basic leading calculation: current position + velocity * prediction time
        Vector3 predictedPosition = player.position + (Vector3)playerRb.linearVelocity * predictionTime;

        // Clamp the leading distance
        Vector3 leadOffset = predictedPosition - player.position;
        if (leadOffset.magnitude > maxLeadDistance)
        {
            leadOffset = leadOffset.normalized * maxLeadDistance;
            predictedPosition = player.position + leadOffset;
        }

        return predictedPosition;
    }

    void onAttack()
    {
        PlaySound(attackSound);
        fireBallLauncher.FireAtLeadingTarget(leadingTarget);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, soundVolume);
        }
    }

    public void DrawLeadingTargetSquare(Vector2 leadingTarget, Color color, float size = 1f)
    {
        if (leadingTarget == null) return;

        Vector2 center = leadingTarget;
        float halfSize = size * 0.5f;

        // Calculate square corners
        Vector2 topLeft = center + new Vector2(-halfSize, halfSize);
        Vector2 topRight = center + new Vector2(halfSize, halfSize);
        Vector2 bottomRight = center + new Vector2(halfSize, -halfSize);
        Vector2 bottomLeft = center + new Vector2(-halfSize, -halfSize);

        // Draw the square
        Debug.DrawLine(topLeft, topRight, color);
        Debug.DrawLine(topRight, bottomRight, color);
        Debug.DrawLine(bottomRight, bottomLeft, color);
        Debug.DrawLine(bottomLeft, topLeft, color);

        // Optional: Draw center cross
        Debug.DrawLine(center + Vector2.left * halfSize, center + Vector2.right * halfSize, color);
        Debug.DrawLine(center + Vector2.down * halfSize, center + Vector2.up * halfSize, color);
    }
}
