using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    
    public void FireAtLeadingTarget(Vector3 leadingTargetPosition)
    {
        // Spawn projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        // Calculate direction to leading target
        Vector3 direction = (leadingTargetPosition - firePoint.position).normalized;
        
        // Add the projectile movement component
        ProjectileMovement movement = projectile.GetComponent<ProjectileMovement>();
        if (movement == null)
            movement = projectile.AddComponent<ProjectileMovement>();
            
        movement.Initialize(direction, projectileSpeed);
    }
}

public class ProjectileMovement : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    
    public void Initialize(Vector3 dir, float spd)
    {
        direction = dir;
        speed = spd;
        
        // Point the projectile in the right direction
        transform.right = direction; // Assumes projectile sprite faces right
    }
    
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        
        // Optional: Destroy after time or distance
        Destroy(gameObject, 5f);
    }
}