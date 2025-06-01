using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            print("Player has entered the door trigger area.");
        } else {
            print("An object other than the player has entered the door trigger area.");
        }
    }
}
