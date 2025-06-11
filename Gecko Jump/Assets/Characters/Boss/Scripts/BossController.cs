using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

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

    [Header("Win Screen")]
    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private AudioClip winMusic;
    [SerializeField] private GameObject winUi;
    [SerializeField] private GameObject mainUi;
    [SerializeField] private CinemachineCamera bossCamera;
    [SerializeField] private PlayerInput playerInput;


    private ProjectileLauncher fireBallLauncher;

    private Animator animator;

    private bool isDead = false;

    private Coroutine attackCoroutine;

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
        attackCoroutine = StartCoroutine(AttackPeriod());
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

         // Stop current attack cycle and restart with shorter delay
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        PlaySound(hitSound);
    }

    public void BossHitEnd()
    {
        isInHitStun = false;

        attackCoroutine = StartCoroutine(AttackAfterHit());
    }

    IEnumerator AttackAfterHit()
    {
        yield return new WaitForSeconds(1f); // Shorter delay after being hit
        
        // Resume normal attack pattern
        attackCoroutine = StartCoroutine(AttackPeriod());
    }

    public void BossDie()
    {
        mainMusic.Stop();

        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
        gameObject.GetComponent<EnemyWaypointPatrol>().moveSpeed = 0;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;
        rb.includeLayers |= LayerMask.GetMask("Platforms");

        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        mainUi.SetActive(false);

        PlaySound(deathSound);

        playerInput.enabled = false;
        bossCamera.Priority = 11;

        StartCoroutine(WinScreen());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platforms"))
        {
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        }    
    }

    IEnumerator WinScreen()
    {
        yield return new WaitForSeconds(3);

        winUi.SetActive(true);
        mainMusic.resource = winMusic;
        mainMusic.Play();
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
