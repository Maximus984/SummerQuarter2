using UnityEngine;

public class SpeedTokenSpawner : MonoBehaviour
{
    private float nextTokenScore = 35f;
    private PlayerController player;

    private void Start() => player = GetComponent<PlayerController>();

    private void Update()
    {
        if (player == null || player.CurrentScore < nextTokenScore) return;
        nextTokenScore += 55f;

        GameObject token = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        token.name = "Speed Token";
        token.transform.position = transform.position + Vector3.forward * 22f + Vector3.right * Random.Range(-3.5f, 3.5f) + Vector3.up;
        token.transform.localScale = new Vector3(.8f, .35f, .8f);
        token.GetComponent<Renderer>().material.color = new Color(1f, .45f, .05f);
        token.GetComponent<Collider>().isTrigger = true;
        token.AddComponent<SpeedToken>();
        Destroy(token, 12f);
    }
}

public class SpeedToken : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        player.CollectSpeedToken();
        Destroy(gameObject);
    }
}
