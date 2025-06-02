using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DoorChecker"))
        {
            print("Player has entered the door trigger area.");

            SceneManager.LoadScene(sceneToLoad);

        }
        else
        {
            print("An object other than the player has entered the door trigger area.");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DoorChecker"))
        {
            print("Player has exited the door trigger area.");
        }
        else
        {
            print("An object other than the player has exited the door trigger area.");
        }
    }
}
