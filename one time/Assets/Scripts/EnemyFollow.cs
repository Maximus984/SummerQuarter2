using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;

    private void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is not assigned for EnemyFollow script.");
            return;
        }
        //update the destination of the enemy to the player's current position 
        agent.SetDestination(target.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
         Destroy(collision.gameObject);
         GameManager.Instance.GameOver();
        }
    }
}