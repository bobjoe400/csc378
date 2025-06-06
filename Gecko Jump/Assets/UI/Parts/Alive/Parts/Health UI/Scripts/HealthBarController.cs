using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private PlayerStats playerStats; // Reference to your ScriptableObject

    void Start() 
    {
        healthSlider = GetComponent<Slider>();
        SetupHealthBar();
    }

    void SetupHealthBar()
    {
        if (playerStats != null && healthSlider != null)
        {
            healthSlider.maxValue = playerStats.maxHealth;
            healthSlider.value = playerStats.health;
        }
    }

    void Update()
    {
        // Continuously update the health bar in case the player stats change
        if (playerStats != null && healthSlider != null)
        {
            if (healthSlider.value != playerStats.health)
            {
                healthSlider.value = playerStats.health;
            }
        }
    }
}