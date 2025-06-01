using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 3;

    [SerializeField] private GameObject lockedDoor;
    [SerializeField] private GameObject unlockedDoor;
    
    // Add Animator reference
    [SerializeField] private Animator animator;
    
    // Add hit animation duration (time before returning to idle)
    [SerializeField] private float hitStunDuration = 0.3f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float soundVolume = 1.0f;
    
    // Track if we're currently in hit stun
    private bool isInHitStun = false;
    
    private void Start()
    {
        // Get animator if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    public void TakeDamage(int amount, Vector2 knockbackForce)
    {
        health -= amount;
        
        // Play hit animation
        if (animator != null && !isInHitStun)
        {
            animator.SetTrigger("Hit");
            StartCoroutine(HitStunRoutine());
        }
        
        // Apply knockback
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Reset velocity for consistent knockback
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        }
        
        // Check if should die
        if (health <= 0)
        {
            Die();
        }
    }
    
    // Prevent multiple hit reactions in quick succession
    private System.Collections.IEnumerator HitStunRoutine()
    {
        PlaySound(hitSound);
        isInHitStun = true;
        yield return new WaitForSeconds(hitStunDuration);
        isInHitStun = false;
    }
    
    private void Die()
    {
        // Play death animation if available
        if (animator != null )
        {
            animator.SetTrigger("Death");
            // Wait for animation before destroying
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }

        if (lockedDoor != null)
        {
            lockedDoor.SetActive(false);
        }
        
        if (unlockedDoor != null)
        {
            unlockedDoor.SetActive(true);
        }

        PlaySound(deathSound);
    }

    // Generic sound player method
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, soundVolume);
        }
    }
}