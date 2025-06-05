using NUnit.Framework.Constraints;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Player Health")]
    public int health = 5;
    [SerializeField] public int maxHealth = 5;
}
