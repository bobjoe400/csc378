using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Game/Player Settings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Basic Movement")]
    [Range(1f, 20f)]
    public float speed = 8f;
    
    [Range(5f, 30f)]
    public float jumpingPower = 16f;
    
    [Header("Wall Jump Settings")]
    [Range(5f, 30f)]
    public float wallJumpPower = 16f;  // Vertical force
    
    [Range(1f, 15f)]
    public float wallJumpDirectionPower = 8f;  // Horizontal push away from wall
    
    [Range(0.05f, 0.5f)]
    public float wallDetectionDistance = 0.1f;  // How far to check for walls
    
    [Range(0.1f, 1f)]
    public float wallJumpCooldown = 0.2f;  // Prevents immediately sticking to walls

    [Header("Collision Detection")]
    [Range(0.1f, 1f)]
    public float longEdgeOffsetFactor = 0.2f;  // Distance from center along long edges
    
    [Range(0.1f, 1f)]
    public float shortEdgeOffsetFactor = 0.2f;  // Distance from center along short edges
    
    [Range(1, 10)]
    public int numberOfRays = 5;  // Number of rays per side
    
    [Range(0.1f, 1f)]
    public float rayLength = 0.2f;  // Length of the rays
    
    [Range(0f, 0.4f)]
    public float edgeInsetFactor = 0.1f;  // How far from the edge corners to inset the rays
    
    [Header("Ray Distribution")]
    [Range(0.1f, 1f)]
    public float longEdgeWidthFactor = 0.8f;  // Width of ray distribution on long edges (1 = full width)
    
    [Range(0.1f, 1f)]
    public float shortEdgeWidthFactor = 0.8f;  // Width of ray distribution on short edges (1 = full width)
    
    [Range(0.5f, 0.95f)]
    public float groundAngleThreshold = 0.7f;  // Cosine of angle for ground detection (0.7 ≈ 45°)
    
    [Range(0.05f, 0.5f)]
    public float wallAngleThreshold = 0.3f;  // Cosine of angle for wall detection (0.3 ≈ 70°)
}