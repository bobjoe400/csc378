using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private GameObject geraldPrefab; // Assign this in the inspector or load from Resources

    [System.Serializable]
    private class AudioSettings
    {
        public AudioSource audioSource;
        public AudioClip respawnSound;

        public float soundVolume = 1.0f;
    }

    [SerializeField] private AudioSettings audioSettings;

    public void InitiateRespawn()
    {
        // Start the coroutine to respawn Gerald
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(geraldPrefab.GetComponent<PlayerController>().settings.respawnTime); // Wait a frame to ensure the scene is fully loaded
        CreateGerald();
        PlaySound(audioSettings.respawnSound);
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSettings.audioSource != null && clip != null)
        {
            audioSettings.audioSource.PlayOneShot(clip, audioSettings.soundVolume);
        }
    }

    public void CreateGerald()
    {
        // Find the GameObject with the tag "Gerald"
        GameObject gerald = GameObject.FindGameObjectWithTag("Player");

        // If not found, instantiate a new one
        if (gerald == null)
        {
            Debug.Log("Gerald not found, spawning a new one.");
            GameObject newGerald = Instantiate(geraldPrefab, transform.position, Quaternion.identity);
            newGerald.transform.rotation = Quaternion.Euler(0, 0, -90); // Set rotation if needed

            cinemachineCamera.Target.TrackingTarget = newGerald.transform;
        }
        else
        {
            Debug.Log("Gerald already exists in the scene.");
        }
    }
}