using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit an enemy
        if (other.CompareTag("Enemy"))
        {
            // Get enemy component
            EnemyController enemy;
            if ((enemy = other.GetComponent<EnemyController>()) != null)
            {
                // Apply damage and knockback
                enemy.TakeDamage(damage);
                return;
            }

            BossController boss;
            if ((boss = other.GetComponent<BossController>()) != null)
            {
                boss.TakeDamage(damage);
                return;
            }
        }
    }
}