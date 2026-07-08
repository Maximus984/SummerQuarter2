using UnityEngine;

public class sectontrigger : MonoBehaviour
{
    // stores the prefab of the road tile to spawm in front of the player 

    [SerializeField] private GameObject roadSection;
    [SerializeField] private float spawnOffset = 18f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        // check if the hit object is the olayer or not 
        if (player != null)
        {
            Instantiate(roadSection, new UnityEngine.Vector3(0, 0, transform.position.z + spawnOffset), Quaternion.identity);

            // Destroy this trigger to avoid multiple triggers 
            Destroy(this);
        }

        // spawn the naxt road section

    }
}
