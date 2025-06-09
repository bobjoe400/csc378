using System.Collections;
using JetBrains.Annotations;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss Stats")]
    [SerializeField] private PlayerStats bossStats;

    // Track if we're currently in hit stun
    private bool isInHitStun = false;

    [Header("Leading Settings")]
    public float predictionTime = 0.5f;        // How far ahead to predict
    public float maxLeadDistance = 10f;        // Maximum leading distance
    public float smoothingSpeed = 5f;          // How smoothly to update

    [Header("Target")]
    public GameObject player;
    private Transform playerTransform;
    public Vector2 leadingTarget;            // Empty GameObject to represent leading position

    private Rigidbody2D playerRb;
    private Vector3 currentLeadingPos;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float soundVolume = 1f;

    private ProjectileLauncher fireBallLauncher;

    private Animator animator;

    private bool isDead = false;

    void Start()
    {
        bossStats.health = bossStats.maxHealth;

        fireBallLauncher = GetComponent<ProjectileLauncher>();

        playerRb = player.GetComponent<Rigidbody2D>();
        playerTransform = player.GetComponent<Transform>();

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentLeadingPos = playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        if (playerTransform == null)
        {
            playerTransform = player.GetComponent<Transform>();
        }

        if (playerRb == null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }

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

    public void TakeDamage(int amount)
    {
        if (isInHitStun || isDead ) return;

        bossStats.health -= amount;

        if (bossStats.health <= 0 && !isDead)
        {
            isDead = true;

            animator.SetTrigger("Die");
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    public void BossHitStart()
    {
        isInHitStun = true;
        if (animator.GetBool("Attacking"))
        {
            animator.SetBool("Attacking", false);
        }

        PlaySound(hitSound);
    }

    public void BossHitEnd()
    {
        isInHitStun = false;
    }

    public void BossDie()
    {
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
        gameObject.GetComponent<EnemyWaypointPatrol>().moveSpeed = 0;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;
        rb.includeLayers |= LayerMask.GetMask("Platforms");

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        PlaySound(deathSound);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platforms"))
        {
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }    
    }

    Vector3 CalculateLeadingPosition()
    {
        // Basic leading calculation: current position + velocity * prediction time
        Vector3 predictedPosition = playerTransform.position + (Vector3)playerRb.linearVelocity * predictionTime;

        // Clamp the leading distance
        Vector3 leadOffset = predictedPosition - playerTransform.position;
        if (leadOffset.magnitude > maxLeadDistance)
        {
            leadOffset = leadOffset.normalized * maxLeadDistance;
            predictedPosition = playerTransform.position + leadOffset;
        }

        return predictedPosition;
    }

    public void OnBossAttack()
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
