using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private int damage = 1;
    public int Damage => damage;
    [SerializeField] private GameObject lockedDoor;
    [SerializeField] private GameObject unlockedDoor;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float soundVolume = 1.0f;

    // Track if we're currently in hit stun
    private bool isInHitStun = false;
    private bool isDead = false;

    private void Start()
    {
        // Get animator if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0 && !isDead)
        {
            isDead = true;
        }

        // Play hit animation
        if (animator != null && !isInHitStun)
        {
            animator.SetTrigger("Hit");
        }
    }

    public void EnemyHitStart()
    {
        isInHitStun = true;

        if (isDead)
        {

            PlaySound(deathSound);

            return;
        }

        PlaySound(hitSound);
    }

    public void EnemyHitEnd()
    {
        if (isDead)
        {
            Die();

            return;
        }

        isInHitStun = false;
    }

    private void Die()
    {
        if (lockedDoor != null)
        {
            lockedDoor.SetActive(false);
        }

        if (unlockedDoor != null)
        {
            unlockedDoor.SetActive(true);
        }

        Destroy(gameObject);
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