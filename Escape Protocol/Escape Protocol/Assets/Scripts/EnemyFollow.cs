using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    [SerializeField] private float speedIncreaseEveryScore = 25f;
    [SerializeField] private float speedIncreaseAmount = 0.5f;
    [SerializeField] private float catchUpDistance = 12f;
    [SerializeField] private float catchUpSpeedBoost = 3f;
    [SerializeField] private float predictionAtLowDanger = 1f;
    [SerializeField] private float predictionAtHighDanger = 4f;

    private float startingSpeed;
    private float learnedStrafeDirection;
    private float patternConfidence;

    private void Awake()
    {
        startingSpeed = agent.speed;

        if (GetComponent<EnemyVisual>() == null)
        {
            gameObject.AddComponent<EnemyVisual>();
        }
    }

    private void Start()
    {
        if (target == null) return;

        // Start each pursuer behind the runner, spread across three lanes.
        float laneOffset = transform.position.x - 10f;
        Vector3 behindPlayer = target.position + Vector3.back * 12f + Vector3.right * laneOffset;
        agent.Warp(behindPlayer);
    }

    private void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is not assigned for EnemyFollow script.");
            return;
        }

        float difficultyLevel = Mathf.Floor(PlayerController.Instance.CurrentScore / speedIncreaseEveryScore) + (GameManager.CurrentRound - 1) * 3f;
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        float catchUpBoost = distanceToPlayer > catchUpDistance ? catchUpSpeedBoost : 0f;
        float dangerPercent = Mathf.Clamp01(difficultyLevel / 20f);
        float predictionDistance = Mathf.Lerp(predictionAtLowDanger, predictionAtHighDanger, dangerPercent);
        float playerStrafeDirection = PlayerController.Instance.CurrentStrafeDirection;

        // Learn the player's usual left/right movement over a short time.
        learnedStrafeDirection = Mathf.Lerp(learnedStrafeDirection, playerStrafeDirection, Time.deltaTime * 2f);
        if (Mathf.Abs(playerStrafeDirection) > 0.1f)
        {
            patternConfidence = Mathf.Clamp01(patternConfidence + Time.deltaTime * 0.6f);
        }
        else
        {
            patternConfidence = Mathf.Clamp01(patternConfidence - Time.deltaTime);
        }

        float predictedDirection = Mathf.Lerp(playerStrafeDirection, learnedStrafeDirection, patternConfidence * dangerPercent);
        Vector3 predictedPosition = target.position + Vector3.right * predictedDirection * predictionDistance;

        agent.speed = startingSpeed + difficultyLevel * (speedIncreaseAmount + .2f) + catchUpBoost;

        // Aim where the player is moving, not only where they are right now
        agent.SetDestination(predictedPosition);
    }

    private void OnCollisionStay(Collision collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeHit();
        }
    }
}
