using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    [SerializeField] private float speedIncreaseEveryScore = 20f;
    [SerializeField] private float speedIncreaseAmount = 0.7f;
    [SerializeField] private float catchUpDistance = 10f;
    [SerializeField] private float catchUpSpeedBoost = 5f;
    [SerializeField] private float predictionAtLowDanger = 1f;
    [SerializeField] private float predictionAtHighDanger = 4f;

    private float startingSpeed;
    private float learnedStrafeDirection;
    private float patternConfidence;
    private bool unescapableMode;

    public void SetUnescapableMode(bool enabled)
    {
        unescapableMode = enabled;
    }

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
        if (unescapableMode) difficultyLevel += 15f;
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        float catchUpBoost = distanceToPlayer > catchUpDistance ? catchUpSpeedBoost : 0f;
        float dangerPercent = Mathf.Clamp01(difficultyLevel / 20f);
        float predictionDistance = Mathf.Lerp(predictionAtLowDanger, predictionAtHighDanger, dangerPercent);
        float playerStrafeDirection = PlayerController.Instance.CurrentStrafeDirection;

        // Learn the player's usual left/right movement over a short time.
        float learningSpeed = unescapableMode ? 6f : 2f;
        learnedStrafeDirection = Mathf.Lerp(learnedStrafeDirection, playerStrafeDirection, Time.deltaTime * learningSpeed);
        if (Mathf.Abs(playerStrafeDirection) > 0.1f)
        {
            patternConfidence = Mathf.Clamp01(patternConfidence + Time.deltaTime * (unescapableMode ? 2f : .6f));
        }
        else
        {
            patternConfidence = Mathf.Clamp01(patternConfidence - Time.deltaTime);
        }

        float predictedDirection = Mathf.Lerp(playerStrafeDirection, learnedStrafeDirection, patternConfidence * dangerPercent);
        Vector3 predictedPosition = target.position + Vector3.right * predictedDirection * predictionDistance;

        // Start more threatening, then become noticeably faster as the run continues.
        float speed = startingSpeed + 1.2f + difficultyLevel * (speedIncreaseAmount + .3f) + catchUpBoost;
        agent.speed = unescapableMode ? speed * 1.35f : speed;

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
