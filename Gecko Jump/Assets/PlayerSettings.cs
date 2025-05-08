using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Game/Player Settings")]
public class PlayerSettings : ScriptableObject
{
    [Range(1f, 20f)]
    public float speed = 8f;
    
    [Range(5f, 30f)]
    public float jumpingPower = 16f;
}