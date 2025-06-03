using System.Collections;
using UnityEngine;

public class TurnOnThunderDome : MonoBehaviour
{
    public GameObject thunderDome;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(EnableThunderDome()); // Start coroutine to enable the Thunder Dome   
            Destroy(gameObject); // Destroy this trigger object to prevent reactivation
        }
    }

    IEnumerator EnableThunderDome()
    {
        yield return new WaitForSeconds(0.2f); // Optional delay before enabling the Thunder Dome
        thunderDome.SetActive(true);
    }
}
