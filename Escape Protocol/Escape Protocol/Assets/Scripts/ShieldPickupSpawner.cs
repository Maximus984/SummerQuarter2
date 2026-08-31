using UnityEngine;

public class ShieldPickupSpawner : MonoBehaviour
{
    private float nextSpawnScore = 60f;
    private PlayerController player;

    private void Start() => player = GetComponent<PlayerController>();

    private void Update()
    {
        if (player.CurrentScore < nextSpawnScore) return;
        nextSpawnScore += 120f;
        GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pickup.name = "Shield Pickup";
        pickup.transform.position = transform.position + Vector3.forward * 20f + Vector3.right * Random.Range(-3f, 3f) + Vector3.up * 2f;
        pickup.transform.localScale = Vector3.one * 1.5f;
        pickup.GetComponent<Renderer>().material.color = Color.cyan;
        pickup.GetComponent<Collider>().isTrigger = true;
        pickup.AddComponent<ShieldPickup>();
        Destroy(pickup, 15f);
    }
}

public class ShieldPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        player.ActivateShield(5f);
        Destroy(gameObject);
    }
}
