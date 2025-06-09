using System.Collections;
using System.Reflection;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private Collider2D newCameraBounds;
    [SerializeField] private GameObject tempCameraObject;
    [SerializeField] private CinemachineCamera bossCamera;
    [SerializeField] private GameObject boss;
    private BossController bossController;
    private EnemyWaypointPatrol bossPatrol;

    private float bossMoveSpeed = 3.0f;

    [SerializeField] private GameObject blocker;

    [SerializeField] private SpawnPoint spawnPoint;
    [SerializeField] private GameObject newSpawnPoint;

    [SerializeField] GameObject player;

    private PlayerInput playerInput;
    private PlayerController playerController;

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
        int counter = 3;
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
}
