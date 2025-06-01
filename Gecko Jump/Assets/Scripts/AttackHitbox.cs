using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;
    public float knockbackForce = 5f;
    
    // Reference to parent player for access to direction info
    private PlayerController player;
    
    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        // Start disabled - only enable during attack
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit an enemy
        if (other.CompareTag("Enemy"))
        {
            // Get enemy component
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Get direction for knockback (based on player orientation)
                Vector2 knockbackDir = player.visualRight.normalized;
                
                // Apply damage and knockback
                enemy.TakeDamage(damage, knockbackDir * knockbackForce);
            }
        }
    }
}