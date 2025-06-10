using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class BossTrigger : MonoBehaviour
{
    [Header("Camera Binds")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Collider2D newCameraBounds;
    [SerializeField] private GameObject tempCameraObject;
    [SerializeField] private CinemachineCamera bossCamera;

    [Header("Boss Binds")]
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject bossHealthUi;
    private BossController bossController;
    private EnemyWaypointPatrol bossPatrol;
    private float bossMoveSpeed = 3.0f;

    [Header("Tile Binds")]
    [SerializeField] private GameObject blocker;

    [Header("Player Binds")]
    [SerializeField] GameObject player;
    private PlayerInput playerInput;
    private PlayerController playerController;
    [SerializeField] private SpawnPoint spawnPoint;
    [SerializeField] private GameObject newSpawnPoint;

    [Header("Sound Binds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip encounterSound;
    [SerializeField] private float soundVolume = 1.0f;


    void Start()
    {
        bossController = boss.GetComponent<BossController>();
        bossPatrol = boss.GetComponent<EnemyWaypointPatrol>();

        playerInput = player.GetComponent<PlayerInput>();
        playerController = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(StartIntro());
        }
    }

    IEnumerator StartIntro()
    {
        PlaySound(encounterSound);

        bossHealthUi.SetActive(true);

        GetComponent<PolygonCollider2D>().enabled = false;
        blocker.SetActive(true);

        playerInput.enabled = false;
        playerController.visualState.isInvuln = true;

        bossCamera.Priority = 11;

        yield return new WaitForSeconds(1);

        bossController.StartAttacking();

        StartCoroutine(WaitForIntroToFinish());
    }

    IEnumerator WaitForIntroToFinish()
    {
        int counter = 2;
        Animator bossAnimator = boss.GetComponent<Animator>();

        while (counter > 0)
        {
            yield return new WaitUntil(() => bossAnimator.GetBool("Attacking"));
            yield return new WaitUntil(() => !bossAnimator.GetBool("Attacking"));
            counter--;
        }

        StartFight();
    }

    void StartFight()
    {
        bossPatrol.moveSpeed = bossMoveSpeed;

        StartCoroutine(WaitToStartPlayerMovement());
    }

    IEnumerator WaitToStartPlayerMovement()
    {
        playerCamera.Target.TrackingTarget = tempCameraObject.transform;
        playerCamera.Lens.OrthographicSize = 6f;

        yield return new WaitForSeconds(0.5f);

        CinemachineConfiner2D confiner = playerCamera.GetComponent<CinemachineConfiner2D>();
        confiner.BoundingShape2D = newCameraBounds;
        confiner.InvalidateBoundingShapeCache();

        yield return new WaitForSeconds(0.5f);

        playerCamera.Target.TrackingTarget = player.transform;

        spawnPoint.transform.position = newSpawnPoint.transform.position;

        yield return new WaitForSeconds(1f);

        bossCamera.Priority = 9;
        playerInput.enabled = true;

        StartCoroutine(PlayerInvulnBuffer());
    }

    IEnumerator PlayerInvulnBuffer()
    {
        yield return new WaitForSeconds(1f);
        playerController.visualState.isInvuln = false;
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, soundVolume);
        }
    }
}
