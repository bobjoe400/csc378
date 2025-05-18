using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyWaypointPatrol : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool loopPath = true;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointReachedDistance = 0.1f;
    [SerializeField] private float waitTimeAtWaypoints = 0.5f;
    
    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isWaiting = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned to EnemyWaypointPatrol");
            enabled = false;
        }
    }
    
    private void Update()
    {
        if (isWaiting || waypoints.Length == 0) return;
        
        // Get current target waypoint
        Transform currentWaypoint = waypoints[currentWaypointIndex];
        
        // Calculate direction and move towards waypoint
        Vector2 direction = (currentWaypoint.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        
        // Update visuals based on movement direction
        UpdateVisuals(direction);
        
        // Check if waypoint reached
        float distanceToWaypoint = Vector2.Distance(transform.position, currentWaypoint.position);
        if (distanceToWaypoint < waypointReachedDistance)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }
    
    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        
        // Update animation
        if (animator != null)
        {
            animator.SetBool("Moving", false);
        }
        
        yield return new WaitForSeconds(waitTimeAtWaypoints);
        
        // Move to next waypoint
        if (loopPath)
        {
            // Simple looping through array
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            // Ping-pong back and forth
            if (movingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length - 1)
                {
                    movingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex <= 0)
                {
                    movingForward = true;
                }
            }
        }
        
        // Resume movement
        if (animator != null)
        {
            animator.SetBool("Moving", true);
        }
        
        isWaiting = false;
    }
    
    private void UpdateVisuals(Vector2 direction)
    {
        // Flip sprite based on horizontal movement
        if (spriteRenderer != null && Mathf.Abs(direction.x) > 0.1f)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }
    
    // Visualize the path in the editor
    private void OnDrawGizmosSelected()
    {
        if (waypoints.Length < 2) return;
        
        Gizmos.color = Color.cyan;
        
        // Draw path
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i+1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i+1].position);
        }
        
        // Draw loop back if needed
        if (loopPath && waypoints[0] != null && waypoints[waypoints.Length-1] != null)
        {
            Gizmos.DrawLine(waypoints[waypoints.Length-1].position, waypoints[0].position);
        }
        
        // Draw waypoints
        Gizmos.color = Color.yellow;
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null)
                Gizmos.DrawSphere(waypoint.position, 0.2f);
        }
    }

    #if UNITY_EDITOR
    // Add this at the bottom of your EnemyWaypointPatrol class
    [ContextMenu("Create Waypoint")]
    private void CreateWaypoint()
    {
        GameObject newWaypoint = new GameObject("Waypoint " + (waypoints.Length + 1));
        
        // Position the new waypoint near the last one or near the enemy if no waypoints yet
        if (waypoints.Length > 0 && waypoints[waypoints.Length - 1] != null)
        {
            newWaypoint.transform.position = waypoints[waypoints.Length - 1].position + Vector3.right;
        }
        else
        {
            newWaypoint.transform.position = transform.position + Vector3.right;
        }
        
        // Check if we have a parent for organization
        Transform waypointsParent = transform.parent?.Find("Waypoints");
        if (waypointsParent == null)
        {
            // Create a new parent
            GameObject parentObj = new GameObject("Waypoints");
            parentObj.transform.position = transform.position;
            if (transform.parent != null)
                parentObj.transform.SetParent(transform.parent);
            waypointsParent = parentObj.transform;
        }
        
        // Parent the new waypoint
        newWaypoint.transform.SetParent(waypointsParent);
        
        // Add the waypoint to the array
        UnityEditor.Undo.RecordObject(this, "Add Waypoint");
        System.Array.Resize(ref waypoints, waypoints.Length + 1);
        waypoints[waypoints.Length - 1] = newWaypoint.transform;
        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Clear Waypoints")]
    private void ClearWaypoints()
    {
        UnityEditor.Undo.RecordObject(this, "Clear Waypoints");
        waypoints = new Transform[0];
        UnityEditor.EditorUtility.SetDirty(this);
    }
    #endif
}