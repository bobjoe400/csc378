using UnityEngine;

public class TurnOnThunderDome : MonoBehaviour
{
    public GameObject thunderDome;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            thunderDome.SetActive(true); // Activate the Thunder Dome when player enters the trigger
            Destroy(gameObject); // Destroy this trigger object to prevent reactivation
        }
    }
}
